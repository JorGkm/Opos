using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opos
{
    public enum ModoPenalizacion
    {
        SinPenalizacion,
        TresMalUnaBien,
        DosMalUnaBien,
        UnaMalUnaBien
    }
    public class Test
    {
        private Examen _examen;
        private GestorBD _bd;

        public ModoPenalizacion Penalizacion { get; set; } = ModoPenalizacion.SinPenalizacion;
        public List<string> opcionesPenalizacion = new List<string> {
            "Estándar (No restan)",
            "Oposición (3 mal restan 1)",
            "Duro (2 mal restan 1)",
            "Muerte súbita (1 mal resta 1)"
        };
        public List<string>? Temas => _examen.preguntasExamen?
            .Select(p => string.IsNullOrWhiteSpace(p.NombreTema) ? p.Tema : $"{p.Tema} - {p.NombreTema}")
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        public List<Pregunta> PreguntasSeleccionadas { get; private set; }

        public Test(Examen ex, GestorBD bd)
        {
            _examen = ex;
            _bd = bd;
            PreguntasSeleccionadas = ex.preguntasExamen ?? new List<Pregunta>();
        }

        public void Filtrar(string temaElegido)
        {
            if (_examen.preguntasExamen == null) return;

            PreguntasSeleccionadas = _examen.preguntasExamen
                .Where(p => string.IsNullOrWhiteSpace(p.NombreTema)
                    ? p.Tema == temaElegido
                    : $"{p.Tema} - {p.NombreTema}" == temaElegido)
                .ToList();
        }

        public void IniciarExamen()
        {
            if (PreguntasSeleccionadas == null || PreguntasSeleccionadas.Count == 0)
            {
                Console.WriteLine("Error: No hay preguntas seleccionadas para el test.");
                Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                Console.ReadKey();
                return;
            }

            List<decimal> tiempos = [];
            List<Pregunta> preguntasFalladas = new();

            int aciertos = 0;
            int fallos = 0;
            int saltos = 0;

            _examen.cronoExamen.Start();

            for (int i = 0; i < PreguntasSeleccionadas.Count; i++)
            {
                var preg = PreguntasSeleccionadas[i];
                char respuestaUsuario = LeerRespuesta(preg, i + 1, out decimal tiempoPregunta);
                tiempos.Add(tiempoPregunta);

                if (respuestaUsuario == 'S')
                {
                    saltos++;
                    continue;
                }

                if (respuestaUsuario == preg.RespuestaCorrecta)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n¡CORRECTO!");
                    aciertos++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nFALLASTE. La correcta era la {preg.RespuestaCorrecta}");
                    fallos++;
                    preguntasFalladas.Add(preg);
                }
                Console.ResetColor();
                Console.WriteLine("\nPulsa una tecla para la siguiente...");
                Console.ReadKey();
            }

            _examen.cronoExamen.Stop();
            (double nota, double notaSinPen) = CalculoNota(aciertos, fallos, saltos);

            string temaFiltro = PreguntasSeleccionadas.Count < (_examen.preguntasExamen?.Count ?? 0)
                ? PreguntasSeleccionadas[0].Tema + (PreguntasSeleccionadas[0].NombreTema != null ? $" - {PreguntasSeleccionadas[0].NombreTema}" : "")
                : "Todos los temas";

            var resultado = new ExamenResultado
            {
                Fecha = DateTime.Now,
                Tema = temaFiltro,
                TotalPreguntas = aciertos + fallos + saltos,
                Aciertos = aciertos,
                Fallos = fallos,
                Saltos = saltos,
                Nota = nota,
                NotaSinPenalizar = notaSinPen,
                Penalizacion = opcionesPenalizacion[(int)Penalizacion],
                TiempoSegundos = (int)_examen.cronoExamen.Elapsed.TotalSeconds,
                TiempoMedioRespuesta = (double)tiempos.Average()
            };

            _bd.GuardarExamen(resultado, preguntasFalladas);
            MostrarResultadosFinales(aciertos, fallos, saltos, tiempos, nota, notaSinPen);
        }

        private char LeerRespuesta(Pregunta preg, int numeroActual, out decimal tiempoFinal)
        {
            int opcionSeleccionada = 0;
            List<string> opciones = new(preg.Opciones.Select((o, i) => $"{(char)('A' + i)}) {o}").ToList());
            opciones.Add("[Saltar]");

            bool respondido = false;
            char respuesta = ' ';
            Stopwatch timer = new();
            timer.Start();

            while (!respondido)
            {
                Console.Clear();
                Console.WriteLine($"Tema: {preg.Tema}{(preg.NombreTema != null ? $" - {preg.NombreTema}" : "")} | Pregunta Nº {numeroActual}/{PreguntasSeleccionadas.Count}");
                Console.WriteLine(new string('-', 50));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{preg.Enunciado}\n");
                Console.ResetColor();

                for (int i = 0; i < opciones.Count; i++)
                {
                    if (i == opcionSeleccionada)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  > {opciones[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"    {opciones[i]}");
                    }
                }

                Console.WriteLine("\n↑↓ Navegar | Enter Confirmar | Espacio Saltar | A/B/C/D Selección directa");

                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                        opcionSeleccionada = (opcionSeleccionada == 0) ? opciones.Count - 1 : opcionSeleccionada - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        opcionSeleccionada = (opcionSeleccionada == opciones.Count - 1) ? 0 : opcionSeleccionada + 1;
                        break;
                    case ConsoleKey.Enter:
                        if (opcionSeleccionada < opciones.Count - 1)
                        {
                            respuesta = (char)('A' + opcionSeleccionada);
                        }
                        else
                        {
                            respuesta = 'S';
                        }
                        respondido = true;
                        break;
                    case ConsoleKey.Spacebar:
                        respuesta = 'S';
                        respondido = true;
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.B:
                    case ConsoleKey.C:
                    case ConsoleKey.D:
                        respuesta = char.ToUpper(tecla.KeyChar);
                        respondido = true;
                        break;
                }
            }

            timer.Stop();
            tiempoFinal = (decimal)timer.Elapsed.TotalSeconds;
            return respuesta;
        }


        private void MostrarResultadosFinales(int a, int f, int s, List<decimal> tmpRespuestas, double nota, double notaSinPen)
        {
            Console.Clear();
            Console.WriteLine("======= EXAMEN FINALIZADO =======");
            Console.WriteLine($"Aciertos: {a}");
            Console.WriteLine($"Fallos: {f}");
            Console.WriteLine($"Abstenciones: {s}");
            Console.WriteLine($"Puntuación sin penalizar: {notaSinPen:F2} de {a + f + s} preguntas");
            Console.WriteLine($"Nota final: {nota:N3}");
            Console.WriteLine($"Tiempo: {_examen.cronoExamen.Elapsed:mm\\:ss}");
            Console.WriteLine($"Tiempo de respuesta: {tmpRespuestas.Average():N2} segundos");
            Console.WriteLine("=================================");
            Console.WriteLine($"\n[!] Resultados guardados en base de datos");
            Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
            Console.ReadKey(true);
        }

        private (double nCalculada, double nSinPenalizar) CalculoNota(int a, int f, int s)
        {
            double resta = Penalizacion switch
            {
                ModoPenalizacion.TresMalUnaBien => f / 3.0,
                ModoPenalizacion.DosMalUnaBien => f / 2.0,
                ModoPenalizacion.UnaMalUnaBien => f,
                _ => 0
            };

            double notaNeto = a - resta;
            int totalPreguntas = a + f + s;

            double notaSinPenalizar = ((double)a / totalPreguntas) * 10;
            double notaFinal = (notaNeto / totalPreguntas) * 10;
            if (notaFinal < 0) notaFinal = 0;
            return (notaFinal, notaSinPenalizar);
        }
    }
}
