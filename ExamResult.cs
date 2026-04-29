namespace Opos;

public class ExamResult
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
    public int TotalQuestions { get; set; }
    public int Correct { get; set; }
    public int Wrong { get; set; }
    public int Skipped { get; set; }
    public double Score { get; set; }
    public double RawScore { get; set; }
    public string PenaltyMode { get; set; } = string.Empty;
    public int TimeSeconds { get; set; }
    public double AverageAnswerTime { get; set; }
}
