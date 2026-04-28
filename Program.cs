using System.Threading.Tasks;

namespace Opos;

class Program
{
    static async Task Main(string[] args)
    {
        MenuInicio oposMenu = new();
        Importador miImportador = new();
        List<Pregunta> poolPreguntas = new();
        bool buclePrincipal = true;
        while (buclePrincipal)
        {
            Opciones seleccion = await oposMenu.CargarMenu();

            switch (seleccion)
            {
                case Opciones.Cargar:
                    string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "preguntas.txt");
                    poolPreguntas = await miImportador.CargarDesdeTexto(ruta);
                    Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                    Console.ReadKey();
                    break;

                case Opciones.Comenzar:
                    if (poolPreguntas.Count > 0)
                    {
                        Examen nuevoExamen = new Examen(poolPreguntas);
                        Test testActual = new(nuevoExamen);
                        if (testActual.Temas != null && testActual.Temas?.Count > 1)
                        {
                            List<string> opcionesMenu = new List<string> { "TODOS LOS TEMAS" };
                            opcionesMenu.AddRange(testActual.Temas);
                            Console.WriteLine("Se han detectado varios Temas. ¿Qué quieres hacer?");
                            string seleccionUsuario =  MostrarOpciones(opcionesMenu);

                            if (seleccionUsuario != "TODOS LOS TEMAS")
                            {
                                testActual.Filtrar(seleccionUsuario);
                            }
                        }
                        Console.Clear();
                        Console.WriteLine("Selecciona el sistema de puntuación:");
                        string penalizacionElegida =  MostrarOpciones(testActual.opcionesPenalizacion);

                        testActual.Penalizacion = penalizacionElegida switch
                        {
                            "Oposición (3 mal restan 1)" => ModoPenalizacion.TresMalUnaBien,
                            "Duro (2 mal restan 1)" => ModoPenalizacion.DosMalUnaBien,
                            "Muerte súbita (1 mal resta 1)" => ModoPenalizacion.UnaMalUnaBien,
                            _ => ModoPenalizacion.SinPenalizacion
                        };
                        testActual.IniciarExamen();
                    }
                    else
                    {
                        Console.WriteLine("\n[!] Primero debes cargar las preguntas.");
                        Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                        Console.ReadKey();
                    }
                    break;

                case Opciones.Salir:
                    buclePrincipal = false;
                    break;
            }
        }
    }
    private static string MostrarOpciones(List<string> listaOpc)
    {
        int indiceSeleccionado = 0;
        ConsoleKey tecla;
        bool elegido = false;
        do
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("\n Use las flechas [↑/↓] para navegar y [Enter] para seleccionar:\n");
            for (int i = 0; i < listaOpc.Count; i++)
            {
                if (i == indiceSeleccionado)
                {
                    // Resaltamos la opción seleccionada
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

            // Leemos la tecla 
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
