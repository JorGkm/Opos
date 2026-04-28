using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Opos
{
    public enum ModoPenalizacion
    {
        SinPenalizacion, // Las malas no restan
        TresMalUnaBien,  // (resta 0.33)
        DosMalUnaBien,   // (resta 0.50)
        UnaMalUnaBien    // (resta 1.00)
    }
    public class Test
    {
        private Examen _examen;

        public ModoPenalizacion Penalizacion { get; set; } = ModoPenalizacion.SinPenalizacion;
        public List<string> opcionesPenalizacion = new List<string> {
            "Estándar (No restan)",
            "Oposición (3 mal restan 1)",
            "Duro (2 mal restan 1)",
            "Muerte súbita (1 mal resta 1)"
        };
        public List<string>? Temas => _examen.preguntasExamen?
            .Select(p => p.Tema)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        public List<Pregunta> PreguntasSeleccionadas { get; private set; }

        public Test(Examen ex)
        {
            _examen = ex;
            // Primero, las seleccionadas todas
            PreguntasSeleccionadas = ex.preguntasExamen ?? new List<Pregunta>();
        }

        // Método para filtrar la lista antes de empezar
        public void Filtrar(string temaElegido)
        {
            if (_examen.preguntasExamen == null) return;

            PreguntasSeleccionadas = _examen.preguntasExamen
                .Where(p => p.Tema == temaElegido)
                .ToList();
        }

        // Método para lanzar el bucle de preguntas
        public void IniciarExamen()
        {
            if (PreguntasSeleccionadas == null || PreguntasSeleccionadas.Count == 0)
            {
                Console.WriteLine("Error: No hay preguntas seleccionadas para el test.");
                Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                Console.ReadKey();
                return;
            }

            Stopwatch TiempoRespuesta = new();
            Decimal TmpActual = 0;
            List<decimal> Tiempos = [];
            if (PreguntasSeleccionadas.Count == 0) return;

            int aciertos = 0;
            int fallos = 0;
            int saltos = 0;

            _examen.cronoExamen.Start();

            foreach (var preg in PreguntasSeleccionadas)
            {
                Console.Clear();
                Console.WriteLine($"Tema: {preg.Tema} | Pregunta Nº {preg.NumeroPregunta}/{PreguntasSeleccionadas.Count}");
                Console.WriteLine(new string('-', 30));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{preg.Enunciado}\n");
                Console.ResetColor();

                char letra = 'A';
                foreach (var opcion in preg.Opciones)
                {
                    Console.WriteLine($"{letra}) {opcion}");
                    letra++;
                }
                //Le va sumando al valor interno unicode y suma hasta completar las opciones

                Console.Write("\nTu respuesta (A, B, C, D o 'S' para Saltar): ");

                TiempoRespuesta.Start();

                char respuestaUsuario = char.ToUpper(Console.ReadKey(true).KeyChar);

                TiempoRespuesta.Stop();
                TmpActual = (decimal)TiempoRespuesta.Elapsed.TotalSeconds;
                Tiempos.Add(TmpActual);

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
                }
                Console.ResetColor();
                Console.WriteLine("\nPulsa una tecla para la siguiente...");
                Console.ReadKey();
            }

            _examen.cronoExamen.Stop();
            MostrarResultadosFinales(aciertos, fallos, saltos, Tiempos);
        }


        private void MostrarResultadosFinales(int a, int f, int s, List<Decimal> tmpRespuestas)
        {
            (double notaCalculada, double notaSinPenzalizar) = CalculoNota(a, f, s);
            Console.Clear();
            Console.WriteLine("======= EXAMEN FINALIZADO =======");
            Console.WriteLine($"Aciertos: {a}");
            Console.WriteLine($"Fallos: {f}");
            Console.WriteLine($"Abstenciones: {s}");
            Console.WriteLine($"Puntuación sin penalizar: {notaSinPenzalizar:F2} de {a + f + s} preguntas");
            Console.WriteLine($"Nota final: {notaCalculada:N3}"); // Nota sobre 10
            Console.WriteLine($"Tiempo: {_examen.cronoExamen.Elapsed:mm\\:ss}");
            Console.WriteLine($"Tiempo de respuesta: {tmpRespuestas.Average():N2} segundos");
            Console.WriteLine("=================================");
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

            // Sobre 10
            double notaFinal = (notaNeto / totalPreguntas) * 10;
            if (notaFinal < 0) notaFinal = 0;
            return (notaFinal, notaFinal);
        }
    }
}