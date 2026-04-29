using System;
using System.Collections.Generic;

namespace Opos;

public static class MenuUI
{
    public static string ShowSelectionMenu(List<string> options, string? banner = null)
    {
        int selectedIndex = 0;
        ConsoleKey key;
        bool selected = false;
        do
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();

            if (!string.IsNullOrEmpty(banner))
                Console.WriteLine(banner);

            Console.WriteLine("\n Use arrow keys [↑/↓] to navigate and [Enter] to select:\n");
            for (int i = 0; i < options.Count; i++)
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
                selectedIndex = (selectedIndex == 0) ? options.Count - 1 : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == options.Count - 1) ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                selected = true;
            }

        } while (!selected);

        return options[selectedIndex];
    }
}
