namespace Opos;

public class ExamenResultado
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Tema { get; set; }
    public int TotalPreguntas { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Saltos { get; set; }
    public double Nota { get; set; }
    public double NotaSinPenalizar { get; set; }
    public string Penalizacion { get; set; } = string.Empty;
    public int TiempoSegundos { get; set; }
    public double TiempoMedioRespuesta { get; set; }
}
