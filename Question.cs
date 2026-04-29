namespace Opos;

public class Question
{
    public string Topic { get; set; } = "General";
    public string? TopicName { get; set; }
    public int QuestionNumber { get; set; }
    public string? Text { get; set; }
    public List<string> Options { get; set; }
    public char CorrectAnswer { get; set; }

    public Question()
    {
        Options = new List<string>();
    }
}
