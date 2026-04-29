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
        I18n.Initialize();

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
                    Console.WriteLine(I18n.T("press_key_continue"));
                    Console.ReadKey();
                    break;

                case MenuOption.Start:
                    if (questionPool.Count > 0)
                    {
                        List<string> examModes = new List<string> {
                            I18n.T("mode_standard"),
                            I18n.T("mode_review")
                        };
                        string chosenMode = MenuUI.ShowSelectionMenu(examModes);

                        TestSession? testSession;

                        if (chosenMode == I18n.T("mode_review"))
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
                                List<string> topicOptions = new List<string> { I18n.T("prompt_all_topics") };
                                topicOptions.AddRange(testSession.Topics);
                                Console.WriteLine(I18n.T("prompt_topics"));
                                string userSelection = MenuUI.ShowSelectionMenu(topicOptions);

                                if (userSelection != I18n.T("prompt_all_topics"))
                                {
                                    testSession.FilterByTopic(userSelection);
                                }
                            }
                        }

                        Console.Clear();
                        List<string> shuffleOptions = new List<string> {
                            I18n.T("shuffle_none"),
                            I18n.T("shuffle_questions"),
                            I18n.T("shuffle_options"),
                            I18n.T("shuffle_both")
                        };
                        Console.WriteLine(I18n.T("prompt_shuffle"));
                        string chosenShuffle = MenuUI.ShowSelectionMenu(shuffleOptions);

                        testSession.ShuffleQuestions = chosenShuffle.Contains("question", StringComparison.OrdinalIgnoreCase);
                        testSession.ShuffleOptions = chosenShuffle.Contains("option", StringComparison.OrdinalIgnoreCase);

                        Console.Clear();
                        Console.WriteLine(I18n.T("prompt_penalty"));
                        string chosenPenalty = MenuUI.ShowSelectionMenu(testSession.GetPenaltyOptions());

                        testSession.Penalty = chosenPenalty switch
                        {
                            var s when s == I18n.T("penalty_opposition") => PenaltyMode.ThreeWrongOneRight,
                            var s when s == I18n.T("penalty_hard") => PenaltyMode.TwoWrongOneRight,
                            var s when s == I18n.T("penalty_sudden_death") => PenaltyMode.OneWrongOneRight,
                            _ => PenaltyMode.NoPenalty
                        };
                        testSession.StartExam();
                    }
                    else
                    {
                        Console.WriteLine(I18n.T("error_no_questions"));
                        Console.WriteLine(I18n.T("press_key_continue"));
                        Console.ReadKey();
                    }
                    break;

                case MenuOption.Statistics:
                    StatisticsView.Show(db);
                    Console.WriteLine(I18n.T("press_key_continue"));
                    Console.ReadKey();
                    break;

                case MenuOption.Instructions:
                    ShowInstructions();
                    Console.WriteLine(I18n.T("press_key_continue"));
                    Console.ReadKey();
                    break;

                case MenuOption.Language:
                    SelectLanguage();
                    Console.WriteLine(I18n.T("press_key_continue"));
                    Console.ReadKey();
                    break;

                case MenuOption.Exit:
                    running = false;
                    break;
            }
        }
    }

    private static void SelectLanguage()
    {
        Console.Clear();
        Console.WriteLine("  Select language / Selecciona idioma\n");

        bool selected = false;
        int selectedIndex = I18n.CurrentLanguage == "en" ? 0 : 1;

        while (!selected)
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("  Select language / Selecciona idioma\n");

            string[] langs = { "en", "es" };
            for (int i = 0; i < langs.Length; i++)
            {
                string check = (i == selectedIndex) ? I18n.T("checkbox_checked") : I18n.T("checkbox_unchecked");
                string name = I18n.GetLanguageDisplayName(langs[i]);
                Console.WriteLine($"  {check} {name}");
            }

            Console.WriteLine("\n  ↑↓ Navigate | Enter Confirm");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = selectedIndex == 0 ? 1 : 0;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = selectedIndex == 1 ? 0 : 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selected = true;
                I18n.SetLanguage(langs[selectedIndex]);
            }
        }
    }

    private static TestSession? PrepareReview(List<Question> pool, DatabaseManager db)
    {
        int totalMisses = db.TotalUniqueMisses();
        if (totalMisses == 0)
        {
            Console.WriteLine(I18n.T("error_no_misses"));
            Console.WriteLine(I18n.T("error_no_misses_action"));
            Console.WriteLine(I18n.T("press_key_continue"));
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
            Console.WriteLine(I18n.T("error_misses_no_match", totalMisses));
            Console.WriteLine(I18n.T("error_misses_no_match_action"));
            Console.WriteLine(I18n.T("press_key_continue"));
            Console.ReadKey();
            return null;
        }

        Console.WriteLine(I18n.T("info_review_found", reviewQuestions.Count, totalMisses));
        Console.WriteLine(I18n.T("press_key_continue"));
        Console.ReadKey();

        return new TestSession(reviewQuestions, db);
    }

    private static string PromptFilePath()
    {
        Console.Clear();
        Console.WriteLine(I18n.T("prompt_load_questions"));
        Console.WriteLine(I18n.T("prompt_drag_drop"));
        Console.Write(I18n.T("prompt_default_path"));

        string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "preguntas.txt");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(defaultPath);
        Console.ResetColor();
        Console.WriteLine(I18n.T("prompt_enter_path"));
        Console.Write(I18n.T("prompt_path_label"));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.CursorVisible = true;

        string? input = Console.ReadLine()?.Trim();
        Console.ResetColor();
        return string.IsNullOrWhiteSpace(input) ? defaultPath : input.Trim('"');
    }

    private static void ShowInstructions()
    {
        Console.Clear();
        string fileName = I18n.CurrentLanguage == "es" ? "instrucciones.txt" : "instructions.txt";
        string? instructions = File.ReadAllText(fileName);
        Console.WriteLine(instructions);
    }
}
