using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Opos
{
    public class Examen
    {
        
        public List<Pregunta>? preguntasExamen;
        public Stopwatch cronoExamen;
        public Examen(List<Pregunta> preg)
        {
            cronoExamen = new();
            preguntasExamen = new();
            cronoExamen.Start();
        }
        private async Task MostrarResultados()
        {
            Console.WriteLine("[RESULTADOS]");
            Console.WriteLine(new String ('=', 12));
            Console.WriteLine($"Tiempo transcurrido: {cronoExamen.Elapsed}");
        }
    }
}