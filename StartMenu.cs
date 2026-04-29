using System;
using System.Collections.Generic;

namespace Opos;

public enum MenuOption { Start, Load, Statistics, Instructions, Language, Exit }

public class StartMenu
{
    private const string ConsoleTitle = "OPOS";
    private const string TitleArt = """

         $$$$$$\  $$$$$$$\   $$$$$$\   $$$$$$\
        $$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\
        $$ /  $$ |$$ |  $$ |$$ /  $$ |$$ /  \__|
        $$ |  $$ |$$$$$$$  |$$ |  $$ |\$$$$$$\
        $$ |  $$ |$$  ____/ $$ |  $$ | \____$$\
        $$ |  $$ |$$ |      $$ |  $$ |$$\   $$ |
         $$$$$$  |$$ |       $$$$$$  |\$$$$$$  |
         \______/ \__|       \______/  \______/

    """;

    public MenuOption? Show()
    {
        Console.Title = ConsoleTitle;
        return DisplayMenu() ?? MenuOption.Exit;
    }

    private MenuOption? DisplayMenu()
    {
        int selectedIndex = 0;
        ConsoleKey key;
        bool selected = false;

        do
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine(TitleArt);
            Console.WriteLine(I18n.T("nav_hint"));

            string[] labels = {
                I18n.T("menu_start"),
                I18n.T("menu_load"),
                I18n.T("menu_statistics"),
                I18n.T("menu_instructions"),
                I18n.T("menu_language"),
                I18n.T("menu_exit")
            };

            for (int i = 0; i < labels.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  > {labels[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {labels[i]}  ");
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? labels.Length - 1 : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == labels.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selected = true;
            }

        } while (!selected);

        MenuOption[] options = { MenuOption.Start, MenuOption.Load, MenuOption.Statistics, MenuOption.Instructions, MenuOption.Language, MenuOption.Exit };
        return options[selectedIndex];
    }
}
