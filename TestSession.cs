using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opos;

public enum PenaltyMode
{
    NoPenalty,
    ThreeWrongOneRight,
    TwoWrongOneRight,
    OneWrongOneRight
}

public class TestSession
{
    private Exam _exam;
    private DatabaseManager _db;

    public PenaltyMode Penalty { get; set; } = PenaltyMode.NoPenalty;
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    public bool IsReviewMode { get; set; }

    public List<string> GetPenaltyOptions() => new List<string> {
        I18n.T("penalty_standard"),
        I18n.T("penalty_opposition"),
        I18n.T("penalty_hard"),
        I18n.T("penalty_sudden_death")
    };

    public List<string>? Topics => _exam.Questions?
        .Select(q => string.IsNullOrWhiteSpace(q.TopicName) ? q.Topic : $"{q.Topic} - {q.TopicName}")
        .Distinct()
        .OrderBy(t => t)
        .ToList();

    public List<Question> SelectedQuestions { get; private set; }

    public TestSession(Exam exam, DatabaseManager db)
    {
        _exam = exam;
        _db = db;
        SelectedQuestions = exam.Questions ?? new List<Question>();
    }

    public TestSession(List<Question> reviewQuestions, DatabaseManager db)
    {
        _exam = new Exam(reviewQuestions);
        _db = db;
        IsReviewMode = true;
        SelectedQuestions = reviewQuestions;
    }

    public void FilterByTopic(string chosenTopic)
    {
        if (_exam.Questions == null) return;

        SelectedQuestions = _exam.Questions
            .Where(q => string.IsNullOrWhiteSpace(q.TopicName)
                ? q.Topic == chosenTopic
                : $"{q.Topic} - {q.TopicName}" == chosenTopic)
            .ToList();
    }

    public void StartExam()
    {
        if (SelectedQuestions == null || SelectedQuestions.Count == 0)
        {
            Console.WriteLine(I18n.T("error_no_questions_selected"));
            Console.WriteLine(I18n.T("press_key_continue"));
            Console.ReadKey();
            return;
        }

        if (ShuffleQuestions)
        {
            Random rng = new();
            SelectedQuestions = SelectedQuestions.OrderBy(_ => rng.Next()).ToList();
        }

        List<decimal> times = [];
        List<Question> missedQuestions = new();

        int correct = 0;
        int wrong = 0;
        int skipped = 0;

        _exam.Timer.Start();

        for (int i = 0; i < SelectedQuestions.Count; i++)
        {
            var question = SelectedQuestions[i];
            char userAnswer = ReadAnswer(question, i + 1, out decimal questionTime, out char actualCorrectAnswer);
            times.Add(questionTime);

            if (userAnswer == 'S')
            {
                skipped++;
                continue;
            }

            if (userAnswer == actualCorrectAnswer)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(I18n.T("result_correct"));
                correct++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(I18n.T("result_wrong", actualCorrectAnswer));
                wrong++;
                missedQuestions.Add(question);
            }
            Console.ResetColor();
            Console.WriteLine(I18n.T("press_key_next"));
            Console.ReadKey();
        }

        _exam.Timer.Stop();
        (double score, double rawScore) = CalculateScore(correct, wrong, skipped);

        string topicLabel = IsReviewMode
            ? I18n.T("mode_review")
            : SelectedQuestions.Count < (_exam.Questions?.Count ?? 0)
                ? SelectedQuestions[0].Topic + (SelectedQuestions[0].TopicName != null ? $" - {SelectedQuestions[0].TopicName}" : "")
                : I18n.T("prompt_all_topics");

        string penaltyLabel = Penalty switch
        {
            PenaltyMode.NoPenalty => I18n.T("penalty_standard"),
            PenaltyMode.ThreeWrongOneRight => I18n.T("penalty_opposition"),
            PenaltyMode.TwoWrongOneRight => I18n.T("penalty_hard"),
            PenaltyMode.OneWrongOneRight => I18n.T("penalty_sudden_death"),
            _ => ""
        };

        var result = new ExamResult
        {
            Date = DateTime.Now,
            Topic = topicLabel,
            TotalQuestions = correct + wrong + skipped,
            Correct = correct,
            Wrong = wrong,
            Skipped = skipped,
            Score = score,
            RawScore = rawScore,
            PenaltyMode = penaltyLabel,
            TimeSeconds = (int)_exam.Timer.Elapsed.TotalSeconds,
            AverageAnswerTime = (double)times.Average()
        };

        _db.SaveExam(result, missedQuestions);
        ShowResults(correct, wrong, skipped, times, score, rawScore);
    }

    private char ReadAnswer(Question question, int currentNumber, out decimal finalTime, out char actualCorrectAnswer)
    {
        List<string> optionTexts = new(question.Options);

        if (ShuffleOptions)
        {
            Random rng = new();
            int n = optionTexts.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (optionTexts[k], optionTexts[n]) = (optionTexts[n], optionTexts[k]);
            }
        }

        int correctIndex = optionTexts.IndexOf(question.Options[question.CorrectAnswer - 'A']);
        actualCorrectAnswer = (char)('A' + correctIndex);

        List<string> options = new();
        for (int i = 0; i < optionTexts.Count; i++)
            options.Add($"{(char)('A' + i)}) {optionTexts[i]}");
        options.Add(I18n.T("option_skip"));

        int selectedOption = 0;
        bool answered = false;
        char answer = ' ';
        Stopwatch timer = new();
        timer.Start();

        while (!answered)
        {
            Console.Clear();
            string header = question.TopicName != null
                ? I18n.T("question_header_with_name", question.Topic, question.TopicName, currentNumber, SelectedQuestions.Count)
                : I18n.T("question_header", question.Topic, currentNumber, SelectedQuestions.Count);
            Console.WriteLine(header);
            Console.WriteLine(I18n.T("separator"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{question.Text}\n");
            Console.ResetColor();

            for (int i = 0; i < options.Count; i++)
            {
                if (i == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  > {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"    {options[i]}");
                }
            }

            Console.WriteLine(I18n.T("controls_hint"));

            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedOption = (selectedOption == 0) ? options.Count - 1 : selectedOption - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedOption = (selectedOption == options.Count - 1) ? 0 : selectedOption + 1;
                    break;
                case ConsoleKey.Enter:
                    answer = options[selectedOption][0];
                    answered = true;
                    break;
                case ConsoleKey.Spacebar:
                    answer = 'S';
                    answered = true;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.B:
                case ConsoleKey.C:
                case ConsoleKey.D:
                    answer = char.ToUpper(key.KeyChar);
                    answered = true;
                    break;
            }
        }

        timer.Stop();
        finalTime = (decimal)timer.Elapsed.TotalSeconds;
        return answer;
    }

    private void ShowResults(int correct, int wrong, int skipped, List<decimal> answerTimes, double score, double rawScore)
    {
        Console.Clear();
        Console.WriteLine(I18n.T("exam_completed"));
        if (IsReviewMode)
            Console.WriteLine(I18n.T("exam_review_title"));
        Console.WriteLine(I18n.T("result_correct_label", correct));
        Console.WriteLine(I18n.T("result_wrong_label", wrong));
        Console.WriteLine(I18n.T("result_skipped_label", skipped));
        Console.WriteLine(I18n.T("result_raw_score", rawScore, correct + wrong + skipped));
        Console.WriteLine(I18n.T("result_final_score", score));
        Console.WriteLine(I18n.T("result_time", _exam.Timer.Elapsed.ToString("mm\\:ss")));
        Console.WriteLine(I18n.T("result_avg_time", answerTimes.Average()));
        Console.WriteLine(I18n.T("result_separator"));
        Console.WriteLine(I18n.T("result_saved"));
        Console.WriteLine(I18n.T("press_key_continue"));
        Console.ReadKey(true);
    }

    private (double finalScore, double rawScore) CalculateScore(int correct, int wrong, int skipped)
    {
        double deduction = Penalty switch
        {
            PenaltyMode.ThreeWrongOneRight => wrong / 3.0,
            PenaltyMode.TwoWrongOneRight => wrong / 2.0,
            PenaltyMode.OneWrongOneRight => wrong,
            _ => 0
        };

        double netScore = correct - deduction;
        int totalQuestions = correct + wrong + skipped;

        double rawScore = ((double)correct / totalQuestions) * 10;
        double finalScore = (netScore / totalQuestions) * 10;
        if (finalScore < 0) finalScore = 0;
        return (finalScore, rawScore);
    }
}
