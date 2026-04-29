using System;
using System.Collections.Generic;
using System.Linq;

namespace Opos;

public static class StatisticsView
{
    public static void Show(DatabaseManager db)
    {
        Console.Clear();
        var (totalExams, averageScore, bestScore, worstScore) = db.GetGeneralStats();

        if (totalExams == 0)
        {
            Console.WriteLine("No exams recorded yet.");
            Console.WriteLine("\nTake your first test to start tracking statistics!");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║         STATISTICS SUMMARY              ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine($"\n  Total exams taken: {totalExams}");
        Console.WriteLine($"  Average score:     {averageScore:N3}");
        Console.WriteLine($"  Best score:        {bestScore:N3}");
        Console.WriteLine($"  Worst score:       {worstScore:N3}");

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   EXAM HISTORY (last 20)");
        Console.WriteLine("══════════════════════════════════════════\n");

        var history = db.GetExamHistory(20);
        DisplayExamHistory(history);

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   WEAKEST TOPICS");
        Console.WriteLine("══════════════════════════════════════════\n");

        var weakTopics = db.GetWeakestTopics();
        if (weakTopics.Count > 0)
        {
            foreach (var topic in weakTopics.Take(10))
            {
                Console.Write($"  {topic.Topic,-35} ");
                DisplayFailureBar(topic.FailureRate);
                Console.WriteLine($"  ({topic.WrongAnswers} wrong)");
            }
        }
        else
        {
            Console.WriteLine("  Not enough data.");
        }

        Console.WriteLine("\n\n══════════════════════════════════════════");
        Console.WriteLine("   MOST MISSED QUESTIONS (Top 10)");
        Console.WriteLine("══════════════════════════════════════════\n");

        var missedQuestions = db.GetMostMissedQuestions(10);
        if (missedQuestions.Count > 0)
        {
            for (int i = 0; i < missedQuestions.Count; i++)
            {
                var question = missedQuestions[i];
                Console.WriteLine($"  {i + 1}. [{question.Topic}] {question.QuestionText}");
                Console.WriteLine($"     Missed: {question.TimesMissed} | Correct answer: {question.CorrectAnswer}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("  Not enough data.");
        }
    }

    private static void DisplayExamHistory(List<ExamResult> history)
    {
        Console.WriteLine($"  {"Date",-16} {"Topic",-25} {"Score",-8} {"C/W/S",-10} {"Time",-8}");
        Console.WriteLine($"  {"",-16} {"",-25} {"",-8} {"",-10} {"",-8}");

        foreach (var exam in history)
        {
            ConsoleColor color = exam.Score >= 5 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = color;
            Console.WriteLine($"  {exam.Date:dd/MM/yyyy HH:mm,-16} {TruncateTopic(exam.Topic),-25} {exam.Score:N3,-8} {exam.Correct}/{exam.Wrong}/{exam.Skipped,-10} {TimeSpan.FromSeconds(exam.TimeSeconds):mm\\:ss,-8}");
            Console.ResetColor();
        }
    }

    private static void DisplayFailureBar(double percentage)
    {
        int width = 20;
        int filled = (int)(percentage / 100 * width);

        Console.Write("[");
        for (int i = 0; i < width; i++)
        {
            if (i < filled)
            {
                Console.ForegroundColor = percentage > 30 ? ConsoleColor.Red : ConsoleColor.Yellow;
                Console.Write("█");
                Console.ResetColor();
            }
            else
            {
                Console.Write("░");
            }
        }
        Console.Write($"] {percentage:F1}%");
    }

    private static string TruncateTopic(string? topic)
    {
        if (string.IsNullOrEmpty(topic)) return "General";
        return topic.Length > 25 ? topic[..22] + "..." : topic;
    }
}
