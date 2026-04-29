using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Opos;

public class GestorBD
{
    private readonly string _cadenaConexion;

    public GestorBD()
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "opos.db");
        _cadenaConexion = $"Data Source={dbPath}";
        InicializarBD();
    }

    private void InicializarBD()
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        string crearResultados = """
            CREATE TABLE IF NOT EXISTS examenes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                fecha TEXT NOT NULL,
                tema TEXT,
                total_preguntas INTEGER NOT NULL,
                aciertos INTEGER NOT NULL,
                fallos INTEGER NOT NULL,
                saltos INTEGER NOT NULL,
                nota REAL NOT NULL,
                nota_sin_penalizar REAL NOT NULL,
                penalizacion TEXT NOT NULL,
                tiempo_segundos INTEGER NOT NULL,
                tiempo_medio REAL NOT NULL
            )
            """;

        string crearFallos = """
            CREATE TABLE IF NOT EXISTS fallos (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                examen_id INTEGER NOT NULL,
                numero_pregunta INTEGER NOT NULL,
                enunciado TEXT NOT NULL,
                tema TEXT NOT NULL,
                respuesta_correcta TEXT NOT NULL,
                FOREIGN KEY (examen_id) REFERENCES examenes(id) ON DELETE CASCADE
            )
            """;

        using var cmdResultados = new SqliteCommand(crearResultados, conexion);
        cmdResultados.ExecuteNonQuery();

        using var cmdFallos = new SqliteCommand(crearFallos, conexion);
        cmdFallos.ExecuteNonQuery();
    }

    public void GuardarExamen(ExamenResultado resultado, List<Pregunta> preguntasFalladas)
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();
        try
        {
            string insertExamen = """
                INSERT INTO examenes (fecha, tema, total_preguntas, aciertos, fallos, saltos, nota, nota_sin_penalizar, penalizacion, tiempo_segundos, tiempo_medio)
                VALUES (@fecha, @tema, @total, @aciertos, @fallos, @saltos, @nota, @notaSinPen, @penalizacion, @tiempo, @tiempoMedio)
                """;

            using var cmd = new SqliteCommand(insertExamen, conexion, transaccion);
            cmd.Parameters.AddWithValue("@fecha", resultado.Fecha.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@tema", (object?)resultado.Tema ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", resultado.TotalPreguntas);
            cmd.Parameters.AddWithValue("@aciertos", resultado.Aciertos);
            cmd.Parameters.AddWithValue("@fallos", resultado.Fallos);
            cmd.Parameters.AddWithValue("@saltos", resultado.Saltos);
            cmd.Parameters.AddWithValue("@nota", resultado.Nota);
            cmd.Parameters.AddWithValue("@notaSinPen", resultado.NotaSinPenalizar);
            cmd.Parameters.AddWithValue("@penalizacion", resultado.Penalizacion);
            cmd.Parameters.AddWithValue("@tiempo", resultado.TiempoSegundos);
            cmd.Parameters.AddWithValue("@tiempoMedio", resultado.TiempoMedioRespuesta);
            cmd.ExecuteNonQuery();

            int examenId = Convert.ToInt32(new SqliteCommand("SELECT last_insert_rowid()", conexion).ExecuteScalar());

            string insertFallo = """
                INSERT INTO fallos (examen_id, numero_pregunta, enunciado, tema, respuesta_correcta)
                VALUES (@examenId, @num, @enunciado, @tema, @respuesta)
                """;

            using var cmdFallo = new SqliteCommand(insertFallo, conexion, transaccion);
            foreach (var preg in preguntasFalladas)
            {
                cmdFallo.Parameters.Clear();
                cmdFallo.Parameters.AddWithValue("@examenId", examenId);
                cmdFallo.Parameters.AddWithValue("@num", preg.NumeroPregunta);
                cmdFallo.Parameters.AddWithValue("@enunciado", preg.Enunciado ?? "");
                cmdFallo.Parameters.AddWithValue("@tema", preg.Tema);
                cmdFallo.Parameters.AddWithValue("@respuesta", preg.RespuestaCorrecta.ToString());
                cmdFallo.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public List<ExamenResultado> ObtenerHistorial(int limite = 50)
    {
        var resultados = new List<ExamenResultado>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand("SELECT * FROM examenes ORDER BY fecha DESC LIMIT @limite", conexion);
        cmd.Parameters.AddWithValue("@limite", limite);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            resultados.Add(new ExamenResultado
            {
                Id = reader.GetInt32(0),
                Fecha = DateTime.Parse(reader.GetString(1)),
                Tema = reader.IsDBNull(2) ? null : reader.GetString(2),
                TotalPreguntas = reader.GetInt32(3),
                Aciertos = reader.GetInt32(4),
                Fallos = reader.GetInt32(5),
                Saltos = reader.GetInt32(6),
                Nota = reader.GetDouble(7),
                NotaSinPenalizar = reader.GetDouble(8),
                Penalizacion = reader.GetString(9),
                TiempoSegundos = reader.GetInt32(10),
                TiempoMedioRespuesta = reader.GetDouble(11)
            });
        }

        return resultados;
    }

    public List<FalloTema> ObtenerTemasDebiles()
    {
        var temas = new Dictionary<string, FalloTema>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand("SELECT tema, COUNT(*) as fallos FROM fallos GROUP BY tema ORDER BY fallos DESC", conexion);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string tema = reader.GetString(0);
            int fallos = reader.GetInt32(1);
            temas[tema] = new FalloTema { Tema = tema, Fallos = fallos };
        }

        using var cmdTotal = new SqliteCommand("SELECT tema, COUNT(*) as total FROM examenes WHERE tema IS NOT NULL GROUP BY tema", conexion);
        using var readerTotal = cmdTotal.ExecuteReader();
        while (readerTotal.Read())
        {
            string tema = readerTotal.GetString(0);
            int total = readerTotal.GetInt32(1);
            if (temas.ContainsKey(tema))
                temas[tema].TotalIntentos += total;
        }

        return new List<FalloTema>(temas.Values);
    }

    public List<PreguntaFrecuente> ObtenerPreguntasMasFalladas(int limite = 10)
    {
        var preguntas = new List<PreguntaFrecuente>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand(
            "SELECT enunciado, tema, respuesta_correcta, COUNT(*) as fallos FROM fallos GROUP BY enunciado, tema ORDER BY fallos DESC LIMIT @limite",
            conexion);
        cmd.Parameters.AddWithValue("@limite", limite);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            preguntas.Add(new PreguntaFrecuente
            {
                Enunciado = reader.GetString(0),
                Tema = reader.GetString(1),
                RespuestaCorrecta = reader.GetString(2)[0],
                Fallos = reader.GetInt32(3)
            });
        }

        return preguntas;
    }

    public (int totalExamenes, double notaMedia, double mejorNota, double peorNota) ObtenerEstadisticasGenerales()
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand(
            "SELECT COUNT(*), AVG(nota), MAX(nota), MIN(nota) FROM examenes", conexion);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return (
                reader.GetInt32(0),
                reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                reader.IsDBNull(3) ? 0 : reader.GetDouble(3)
            );
        }

        return (0, 0, 0, 0);
    }

    public List<FalloUnico> ObtenerFallosUnicos()
    {
        var fallos = new List<FalloUnico>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand(
            "SELECT DISTINCT numero_pregunta, enunciado, tema, respuesta_correcta FROM fallos ORDER BY tema, numero_pregunta", conexion);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            fallos.Add(new FalloUnico
            {
                NumeroPregunta = reader.GetInt32(0),
                Enunciado = reader.GetString(1),
                Tema = reader.GetString(2),
                RespuestaCorrecta = reader.GetString(3)[0]
            });
        }

        return fallos;
    }

    public int TotalFallosUnicos()
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var cmd = new SqliteCommand(
            "SELECT COUNT(DISTINCT enunciado) FROM fallos", conexion);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
