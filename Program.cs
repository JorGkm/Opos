using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Opos;

class Program
{
    static async Task Main(string[] args)
    {
        StartMenu oposMenu = new();
        QuestionImporter importer = new();
        DatabaseManager db = new();
        List<Question> questionPool = new();
        bool running = true;
        while (running)
        {
            MenuOption? selection = oposMenu.Show();

            switch (selection)
            {
                case MenuOption.Load:
                    string filePath = PromptFilePath();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        questionPool = await importer.LoadFromText(filePath);
                    }
                    Console.WriteLine("\n[Opos] Press any key to continue");
                    Console.ReadKey();
                    break;

                case MenuOption.Start:
                    if (questionPool.Count > 0)
                    {
                        List<string> examModes = new List<string> { "Standard Exam", "Failed Questions Review" };
                        string chosenMode = MenuUI.ShowSelectionMenu(examModes);

                        TestSession? testSession;

                        if (chosenMode == "Failed Questions Review")
                        {
                            TestSession? reviewSession = PrepareReview(questionPool, db);
                            if (reviewSession == null) break;
                            testSession = reviewSession;
                        }
                        else
                        {
                            Exam newExam = new(questionPool);
                            testSession = new TestSession(newExam, db);

                            if (testSession.Topics != null && testSession.Topics?.Count > 1)
                            {
                                List<string> topicOptions = new List<string> { "ALL TOPICS" };
                                topicOptions.AddRange(testSession.Topics);
                                Console.WriteLine("Multiple topics detected. What do you want to do?");
                                string userSelection = MenuUI.ShowSelectionMenu(topicOptions);

                                if (userSelection != "ALL TOPICS")
                                {
                                    testSession.FilterByTopic(userSelection);
                                }
                            }
                        }

                        Console.Clear();
                        List<string> shuffleOptions = new List<string> {
                            "No shuffling",
                            "Shuffle questions",
                            "Shuffle answer options",
                            "Shuffle both"
                        };
                        Console.WriteLine("How do you want questions to be presented?");
                        string chosenShuffle = MenuUI.ShowSelectionMenu(shuffleOptions);

                        testSession.ShuffleQuestions = chosenShuffle.Contains("questions");
                        testSession.ShuffleOptions = chosenShuffle.Contains("options");

                        Console.Clear();
                        Console.WriteLine("Select scoring system:");
                        string chosenPenalty = MenuUI.ShowSelectionMenu(testSession.PenaltyOptions);

                        testSession.Penalty = chosenPenalty switch
                        {
                            "Opposition (3 wrong deduct 1)" => PenaltyMode.ThreeWrongOneRight,
                            "Hard (2 wrong deduct 1)" => PenaltyMode.TwoWrongOneRight,
                            "Sudden Death (1 wrong deduct 1)" => PenaltyMode.OneWrongOneRight,
                            _ => PenaltyMode.NoPenalty
                        };
                        testSession.StartExam();
                    }
                    else
                    {
                        Console.WriteLine("\n[!] You must load questions first.");
                        Console.WriteLine("\n[Opos] Press any key to continue");
                        Console.ReadKey();
                    }
                    break;

                case MenuOption.Statistics:
                    StatisticsView.Show(db);
                    Console.WriteLine("\n\n[Opos] Press any key to continue");
                    Console.ReadKey();
                    break;

                case MenuOption.Instructions:
                    ShowInstructions();
                    Console.WriteLine("\n[Opos] Press any key to continue");
                    Console.ReadKey();
                    break;

                case MenuOption.Exit:
                    running = false;
                    break;
            }
        }
    }

    private static TestSession? PrepareReview(List<Question> pool, DatabaseManager db)
    {
        int totalMisses = db.TotalUniqueMisses();
        if (totalMisses == 0)
        {
            Console.WriteLine("\nNo missed questions recorded yet.");
            Console.WriteLine("Take an exam first!");
            Console.WriteLine("\n[Opos] Press any key to continue");
            Console.ReadKey();
            return null;
        }

        List<UniqueMiss> uniqueMisses = db.GetUniqueMisses();
        List<Question> reviewQuestions = new();

        foreach (var miss in uniqueMisses)
        {
            Question? match = pool.FirstOrDefault(q =>
                q.Text == miss.QuestionText && q.Topic == miss.Topic);

            if (match != null)
            {
                Question copy = new()
                {
                    Topic = match.Topic,
                    TopicName = match.TopicName,
                    QuestionNumber = match.QuestionNumber,
                    Text = match.Text,
                    Options = new List<string>(match.Options),
                    CorrectAnswer = miss.CorrectAnswer
                };
                reviewQuestions.Add(copy);
            }
        }

        if (reviewQuestions.Count == 0)
        {
            Console.WriteLine($"\nFound {totalMisses} misses, but they don't match the loaded questions.");
            Console.WriteLine("Load the original question file to enable review mode.");
            Console.WriteLine("\n[Opos] Press any key to continue");
            Console.ReadKey();
            return null;
        }

        Console.WriteLine($"\nFound {reviewQuestions.Count} questions for review out of {totalMisses} recorded misses.");
        Console.WriteLine("\n[Opos] Press any key to continue");
        Console.ReadKey();

        return new TestSession(reviewQuestions, db);
    }

    private static string PromptFilePath()
    {
        Console.Clear();
        Console.WriteLine("Enter the path to your questions file (.txt)");
        Console.WriteLine("(You can paste the path or drag and drop the file here)\n");
        Console.Write("Default path: ");

        string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "preguntas.txt");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(defaultPath);
        Console.ResetColor();
        Console.WriteLine("\nPress Enter to use default, or type the path:");
        Console.Write("Path> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.CursorVisible = true;

        string? input = Console.ReadLine()?.Trim();
        Console.ResetColor();
        return string.IsNullOrWhiteSpace(input) ? defaultPath : input.Trim('"');
    }

    private static void ShowInstructions()
    {
        Console.Clear();
        string? instructions = File.ReadAllText("instructions.txt");
        Console.WriteLine(instructions);
    }
}
