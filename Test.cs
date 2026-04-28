using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Opos
{
    public class Test
    {
        private Examen _examen;

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
            Stopwatch TiempoRespuesta = new();
            Decimal TmpActual = 0;
            List<decimal> Tiempos = [];
            if (PreguntasSeleccionadas.Count == 0) return;

            int aciertos = 0;
            int fallos = 0;


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

                Console.Write("\nTu respuesta (A, B, C, D): ");

                TiempoRespuesta.Start();

                char respuestaUsuario = char.ToUpper(Console.ReadKey(true).KeyChar);

                TiempoRespuesta.Stop();
                TmpActual = (decimal)TiempoRespuesta.Elapsed.TotalSeconds;
                Tiempos.Add(TmpActual);

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
            MostrarResultadosFinales(aciertos, fallos, Tiempos);
        }

        private void MostrarResultadosFinales(int a, int f, List<Decimal> tmpRespuestas)
        {
            Console.Clear();
            Console.WriteLine("======= EXAMEN FINALIZADO =======");
            Console.WriteLine($"Aciertos: {a}");
            Console.WriteLine($"Fallos: {f}");
            Console.WriteLine($"Nota: {(double)a / (a + f) * 10:F2}"); // Nota sobre 10
            Console.WriteLine($"Tiempo: {_examen.cronoExamen.Elapsed:mm\\:ss}");
            Console.WriteLine($"Tiempo de respuesta: {tmpRespuestas.Average():N2} segundos");
            Console.WriteLine("=================================");
            Console.ReadKey();
        }
    }
}