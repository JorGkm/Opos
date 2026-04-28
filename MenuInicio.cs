using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Opos
{
    public enum Opciones { Comenzar, Cargar, Instrucciones, Salir }

    public struct MenuInicio
    {

        #region Propiedades Campos
        private static sbyte numOpc = (sbyte)Enum.GetValues(typeof(Opciones)).Length;

        private static string _tituloConsola = "OPOS";
        private static string _titulo = """

             $$$$$$\  $$$$$$$\   $$$$$$\   $$$$$$\  
            $$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\ 
            $$ /  $$ |$$ |  $$ |$$ /  $$ |$$ /  \__|
            $$ |  $$ |$$$$$$$  |$$ |  $$ |\$$$$$$\  
            $$ |  $$ |$$  ____/ $$ |  $$ | \____$$\ 
            $$ |  $$ |$$ |      $$ |  $$ |$$\   $$ |
             $$$$$$  |$$ |       $$$$$$  |\$$$$$$  |
             \______/ \__|       \______/  \______/ 
                                                                     
        """;
        private List<string>? _listaOpciones;
        #endregion
        #region Metodos
        public Opciones? CargarMenu()
        {
            Console.Title = _tituloConsola;
            if (_listaOpciones == null)
            {
                _listaOpciones = new List<string>();
                foreach (string opc in Enum.GetNames(typeof(Opciones)))
                {
                    _listaOpciones.Add(opc);
                }
            }
            Opciones? seleccion =  MostrarOpciones();
            return seleccion ?? Opciones.Salir;
        }

        private Opciones? MostrarOpciones()
        {
            int indiceSeleccionado = 0;
            ConsoleKey tecla;
            bool elegido = false;
            // Obtenemos los nombres de las opciones del Enum que ya tienes
            string[] opciones = Enum.GetNames(typeof(Opciones));

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
                        // Resaltamos la opción seleccionada
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

                // Leemos la tecla 
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

        private void InsertarOpcion(string opcionNueva)
        {
            if (_listaOpciones != null && _listaOpciones.Contains(opcionNueva))
            {
                Console.WriteLine($"La opcion: {opcionNueva}\nYa existe en el menú");
            }
            else _listaOpciones?.Add(opcionNueva);
        }
        #endregion


    }
}