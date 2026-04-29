using System;
using System.Collections.Generic;

namespace Opos
{
    public enum Opciones { Comenzar, Cargar, Instrucciones, Salir }

    public class MenuInicio
    {
        private const string _tituloConsola = "OPOS";
        private const string _titulo = """

             $$$$$$\  $$$$$$$\   $$$$$$\   $$$$$$\  
            $$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\ 
            $$ /  $$ |$$ |  $$ |$$ /  $$ |$$ /  \__|
            $$ |  $$ |$$$$$$$  |$$ |  $$ |\$$$$$$\  
            $$ |  $$ |$$  ____/ $$ |  $$ | \____$$\ 
            $$ |  $$ |$$ |      $$ |  $$ |$$\   $$ |
             $$$$$$  |$$ |       $$$$$$  |\$$$$$$  |
             \______/ \__|       \______/  \______/ 
                                                                     
        """;

        public Opciones? CargarMenu()
        {
            Console.Title = _tituloConsola;
            return MostrarOpciones() ?? Opciones.Salir;
        }

        private Opciones? MostrarOpciones()
        {
            string[] opciones = Enum.GetNames(typeof(Opciones));
            int indiceSeleccionado = 0;
            ConsoleKey tecla;
            bool elegido = false;

            do
            {
                Console.SetCursorPosition(0, 0);
                Console.CursorVisible = false;
                Console.Clear();
                Console.WriteLine(_titulo);
                Console.WriteLine("\n Use las flechas [↑/↓] para navegar y [Enter] para seleccionar:\n");

                for (int i = 0; i < opciones.Length; i++)
                {
                    if (i == indiceSeleccionado)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  > {opciones[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {opciones[i]}  ");
                    }
                }

                tecla = Console.ReadKey(true).Key;

                if (tecla == ConsoleKey.UpArrow)
                {
                    indiceSeleccionado = (indiceSeleccionado == 0) ? opciones.Length - 1 : indiceSeleccionado - 1;
                }
                else if (tecla == ConsoleKey.DownArrow)
                {
                    indiceSeleccionado = (indiceSeleccionado == opciones.Length - 1) ? 0 : indiceSeleccionado + 1;
                }
                else if (tecla == ConsoleKey.Enter)
                {
                    elegido = true;
                }

            } while (!elegido);

            return (Opciones)indiceSeleccionado;
        }
    }
}
