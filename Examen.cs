using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Opos
{
    public struct Examen
    {
        
        public List<Pregunta>? preguntasExamen;
        public Stopwatch cronoExamen;
        public Examen(List<Pregunta> preg)
        {
            cronoExamen = new();
            preguntasExamen = preg;
        }
    }
}