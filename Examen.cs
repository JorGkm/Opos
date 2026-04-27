using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ExamenTestDiputacion;

namespace Opos
{
    public class Examen
    {
        
        private List<string>? respuestasFinal;
        private Stopwatch cronoExamen;
        public Examen()
        {
            cronoExamen = new();
            respuestasFinal = new();
            cronoExamen.Start();
        }
        private async Task MostrarResultados()
        {
            
        }
    }
}