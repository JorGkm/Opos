using System;
using System.Collections.Generic;

namespace Opos
{
    public static class UIHelper
    {
        public static string MostrarOpciones(List<string> listaOpc, string? banner = null)
        {
            int indiceSeleccionado = 0;
            ConsoleKey tecla;
            bool elegido = false;
            do
            {
                Console.SetCursorPosition(0, 0);
                Console.CursorVisible = false;
                Console.Clear();

                if (!string.IsNullOrEmpty(banner))
                    Console.WriteLine(banner);

                Console.WriteLine("\n Use las flechas [↑/↓] para navegar y [Enter] para seleccionar:\n");
                for (int i = 0; i < listaOpc.Count; i++)
                {
                    if (i == indiceSeleccionado)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  > {listaOpc[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {listaOpc[i]}  ");
                    }
                }

                tecla = Console.ReadKey(true).Key;

                if (tecla == ConsoleKey.UpArrow)
                {
                    indiceSeleccionado = (indiceSeleccionado == 0) ? listaOpc.Count - 1 : indiceSeleccionado - 1;
                }
                else if (tecla == ConsoleKey.DownArrow)
                {
                    indiceSeleccionado = (indiceSeleccionado == listaOpc.Count - 1) ? 0 : indiceSeleccionado + 1;
                }
                else if (tecla == ConsoleKey.Enter)
                {
                    elegido = true;
                }

            } while (!elegido);

            return listaOpc[indiceSeleccionado];
        }
    }
}
