using System;
using System.Collections.Generic;

namespace Opos;

public enum MenuOption { Start, Load, Statistics, Instructions, Exit }

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
        string[] options = Enum.GetNames(typeof(MenuOption));
        int selectedIndex = 0;
        ConsoleKey key;
        bool selected = false;

        do
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine(TitleArt);
            Console.WriteLine("\n Use arrow keys [↑/↓] to navigate and [Enter] to select:\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  > {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}  ");
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selected = true;
            }

        } while (!selected);

        return (MenuOption)selectedIndex;
    }
}
