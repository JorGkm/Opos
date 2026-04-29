using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opos
{
    public class Examen
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
