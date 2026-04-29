using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Opos;

public class QuestionImporter
{
    public async Task<List<Question>> LoadFromText(string filePath)
    {
        List<Question> questions = new List<Question>();

        if (!File.Exists(filePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(I18n.T("error_critical_file", filePath));
            Console.ResetColor();
            return questions;
        }

        string fullContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

        string[] parts = fullContent.Split(new string[] { "### RESPUESTAS ###" }, StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(I18n.T("error_format_answers"));
            Console.ResetColor();
        }

        string questionsText = parts[0];
        string answersText = (parts.Length > 1) ? parts[1] : "";

        var lines = questionsText.Split('\n');
        Question? currentQuestion = null;
        string currentTopic = I18n.T("stats_general");
        string? currentTopicName = null;

        Regex questionStartRegex = new Regex(@"^\s*(\d+)[\.\-](?!\d)\s*(.*)");
        Regex optionRegex = new Regex(@"^([a-d])\)\s*(.*)", RegexOptions.IgnoreCase);
        Regex topicRegex = new Regex(@"^TEMA\s+(\d+)\s*[-:—–]?\s*(.*)", RegexOptions.IgnoreCase);

        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            var topicMatch = topicRegex.Match(trimmedLine);
            if (topicMatch.Success)
            {
                currentTopic = I18n.T("topic_prefix", topicMatch.Groups[1].Value);
                currentTopicName = topicMatch.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(currentTopicName)) currentTopicName = null;
                continue;
            }

            var startMatch = questionStartRegex.Match(trimmedLine);
            var optionMatch = optionRegex.Match(trimmedLine);

            if (startMatch.Success)
            {
                if (currentQuestion != null && currentQuestion.Options.Count >= 2)
                    questions.Add(currentQuestion);

                currentQuestion = new Question
                {
                    QuestionNumber = int.Parse(startMatch.Groups[1].Value),
                    Text = startMatch.Groups[2].Value,
                    Topic = currentTopic,
                    TopicName = currentTopicName
                };
            }
            else if (optionMatch.Success && currentQuestion != null)
            {
                currentQuestion.Options.Add(optionMatch.Groups[2].Value);
            }
            else if (currentQuestion != null)
            {
                if (currentQuestion.Options.Count == 0)
                    currentQuestion.Text += " " + trimmedLine;
                else
                    currentQuestion.Options[currentQuestion.Options.Count - 1] += " " + trimmedLine;
            }
        }
        if (currentQuestion != null) questions.Add(currentQuestion);

        Console.WriteLine(I18n.T("info_loaded_file", filePath));
        Console.WriteLine(I18n.T("info_questions_read", questions.Count));

        Dictionary<int, char> answers = new Dictionary<int, char>();

        answersText = answersText.Replace("PREG", "").Replace("RESP", "").Replace("Página", "");

        Regex answerRegex = new Regex(@"(\d+)[\s\r\n]+([A-D])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var matches = answerRegex.Matches(answersText);
        Console.WriteLine(I18n.T("info_answers_detected", matches.Count));

        foreach (Match match in matches)
        {
            int num = int.Parse(match.Groups[1].Value);
            char letter = match.Groups[2].Value.ToUpper()[0];
            if (!answers.ContainsKey(num)) answers.Add(num, letter);
        }

        int answersAssigned = 0;
        foreach (var question in questions)
        {
            if (answers.ContainsKey(question.QuestionNumber))
            {
                question.CorrectAnswer = answers[question.QuestionNumber];
                answersAssigned++;
            }
            else
            {
                question.CorrectAnswer = '?';
            }
        }
        Console.WriteLine(I18n.T("info_answers_linked", answersAssigned));
        return questions;
    }
}
