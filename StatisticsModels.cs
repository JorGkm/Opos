namespace Opos;

public class TopicFailure
{
    public string Topic { get; set; } = string.Empty;
    public int WrongAnswers { get; set; }
    public int TotalAttempts { get; set; }
    public double FailureRate => TotalAttempts > 0 ? (double)WrongAnswers / TotalAttempts * 100 : 0;
}

public class FrequentlyMissedQuestion
{
    public string QuestionText { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public char CorrectAnswer { get; set; }
    public int TimesMissed { get; set; }
}

public class UniqueMiss
{
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public char CorrectAnswer { get; set; }
}
