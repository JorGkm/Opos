namespace Opos;

public class FalloTema
{
    public string Tema { get; set; } = string.Empty;
    public int Fallos { get; set; }
    public int TotalIntentos { get; set; }
    public double PorcentajeFallo => TotalIntentos > 0 ? (double)Fallos / TotalIntentos * 100 : 0;
}

public class PreguntaFrecuente
{
    public string Enunciado { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public char RespuestaCorrecta { get; set; }
    public int Fallos { get; set; }
}
