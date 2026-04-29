using System;

namespace Opos
{
    public class Pregunta
    {
        public string Tema { get; set; } = "General";
        public string? NombreTema { get; set; }
        public int NumeroPregunta { get; set; }
        public string? Enunciado { get; set; }
        public List<string> Opciones { get; set; } 
        public char RespuestaCorrecta { get; set; } 

        public Pregunta()
        {
            Opciones = new List<string>();
        }
    }
}
