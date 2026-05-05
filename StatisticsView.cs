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
            Console.WriteLine(I18n.T("stats_no_exams"));
            Console.WriteLine(I18n.T("stats_first_test"));
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine(I18n.T("stats_title"));
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine(I18n.T("stats_total_exams", totalExams));
        Console.WriteLine(I18n.T("stats_avg_score", averageScore));
        Console.WriteLine(I18n.T("stats_best_score", bestScore));
        Console.WriteLine(I18n.T("stats_worst_score", worstScore));

        Console.WriteLine($"\n\n{I18n.T("stats_divider")}");
        Console.WriteLine(I18n.T("stats_history_title"));
        Console.WriteLine($"{I18n.T("stats_divider")}\n");

        var history = db.GetExamHistory(20);
        DisplayExamHistory(history);

        Console.WriteLine($"\n\n{I18n.T("stats_divider")}");
        Console.WriteLine(I18n.T("stats_weakest_title"));
        Console.WriteLine($"{I18n.T("stats_divider")}\n");

        var weakTopics = db.GetWeakestTopics();
        if (weakTopics.Count > 0)
        {
            foreach (var topic in weakTopics.Take(10))
            {
                Console.Write($"  {topic.Topic,-35} ");
                DisplayFailureBar(topic.FailureRate);
                Console.WriteLine($"  {I18n.T("stats_wrong_label", topic.WrongAnswers)}");
            }
        }
        else
        {
            Console.WriteLine(I18n.T("stats_not_enough_data"));
        }

        Console.WriteLine($"\n\n{I18n.T("stats_divider")}");
        Console.WriteLine(I18n.T("stats_missed_title"));
        Console.WriteLine($"{I18n.T("stats_divider")}\n");

        var missedQuestions = db.GetMostMissedQuestions(10);
        if (missedQuestions.Count > 0)
        {
            for (int i = 0; i < missedQuestions.Count; i++)
            {
                var question = missedQuestions[i];
                Console.WriteLine($"  {i + 1}. [{question.Topic}] {question.QuestionText}");
                Console.WriteLine(I18n.T("stats_missed_label", question.TimesMissed, question.CorrectAnswer));
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine(I18n.T("stats_not_enough_data"));
        }
    }

    private static void DisplayExamHistory(List<ExamResult> history)
    {
        Console.WriteLine(I18n.T("stats_history_subheader"));

        foreach (var exam in history)
        {
            ConsoleColor color = exam.Score >= 5 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = color;
                Console.WriteLine($"  {exam.Date,-16:dd/MM/yyyy HH:mm} {TruncateTopic(exam.Topic),-25} {exam.Score,-8:N3} {exam.Correct}/{exam.Wrong}/{exam.Skipped,-10} {TimeSpan.FromSeconds(exam.TimeSeconds),-8:mm\\:ss}");
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
        if (string.IsNullOrEmpty(topic)) return I18n.T("stats_general");
        return topic.Length > 25 ? topic[..22] + "..." : topic;
    }
}
