using System;
using System.Collections.Generic;
using System.Linq;

namespace Opos;

public static class EstadisticasView
{
    public static void Mostrar(GestorBD bd)
    {
        Console.Clear();
        var (totalExamenes, notaMedia, mejorNota, peorNota) = bd.ObtenerEstadisticasGenerales();

        if (totalExamenes == 0)
        {
            Console.WriteLine("No hay exámenes registrados todavía.");
            Console.WriteLine("\n¡Realiza tu primer test para empezar a ver estadísticas!");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║        RESUMEN ESTADÍSTICO              ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine($"\n  Total exámenes realizados: {totalExamenes}");
        Console.WriteLine($"  Nota media:                {notaMedia:N3}");
        Console.WriteLine($"  Mejor nota:                {mejorNota:N3}");
        Console.WriteLine($"  Peor nota:                 {peorNota:N3}");

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   HISTORIAL DE EXÁMENES (últimos 20)");
        Console.WriteLine("══════════════════════════════════════════\n");

        var historial = bd.ObtenerHistorial(20);
        MostrarTablaHistorial(historial);

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   TEMAS MÁS FALLADOS");
        Console.WriteLine("══════════════════════════════════════════\n");

        var temasDebiles = bd.ObtenerTemasDebiles();
        if (temasDebiles.Count > 0)
        {
            foreach (var tema in temasDebiles.Take(10))
            {
                Console.Write($"  {tema.Tema,-35} ");
                MostrarBarraFallo(tema.PorcentajeFallo);
                Console.WriteLine($"  ({tema.Fallos} fallos)");
            }
        }
        else
        {
            Console.WriteLine("  No hay datos suficientes.");
        }

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   PREGUNTAS MÁS FALLADAS (Top 10)");
        Console.WriteLine("══════════════════════════════════════════\n");

        var preguntasFalladas = bd.ObtenerPreguntasMasFalladas(10);
        if (preguntasFalladas.Count > 0)
        {
            for (int i = 0; i < preguntasFalladas.Count; i++)
            {
                var preg = preguntasFalladas[i];
                Console.WriteLine($"  {i + 1}. [{preg.Tema}] {preg.Enunciado}");
                Console.WriteLine($"     Fallos: {preg.Fallos} | Respuesta correcta: {preg.RespuestaCorrecta}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("  No hay datos suficientes.");
        }
    }

    private static void MostrarTablaHistorial(List<ExamenResultado> historial)
    {
        Console.WriteLine($"  {"Fecha",-12} {"Tema",-25} {"Nota",-8} {"A/F/S",-10} {"Tiempo",-8}");
        Console.WriteLine($"  {"",-12} {"",-25} {"",-8} {"",-10} {"",-8}");

        foreach (var ex in historial)
        {
            ConsoleColor color = ex.Nota >= 5 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = color;
            Console.WriteLine($"  {ex.Fecha:dd/MM/yyyy HH:mm,-12} {TruncarTema(ex.Tema),-25} {ex.Nota:N3,-8} {ex.Aciertos}/{ex.Fallos}/{ex.Saltos,-10} {TimeSpan.FromSeconds(ex.TiempoSegundos):mm\\:ss,-8}");
            Console.ResetColor();
        }
    }

    private static void MostrarBarraFallo(double porcentaje)
    {
        int ancho = 20;
        int llenos = (int)(porcentaje / 100 * ancho);

        Console.Write("[");
        for (int i = 0; i < ancho; i++)
        {
            if (i < llenos)
            {
                Console.ForegroundColor = porcentaje > 30 ? ConsoleColor.Red : ConsoleColor.Yellow;
                Console.Write("█");
                Console.ResetColor();
            }
            else
            {
                Console.Write("░");
            }
        }
        Console.Write($"] {porcentaje:F1}%");
    }

    private static string TruncarTema(string? tema)
    {
        if (string.IsNullOrEmpty(tema)) return "General";
        return tema.Length > 25 ? tema[..22] + "..." : tema;
    }
}
