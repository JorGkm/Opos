using System;
using System.Collections.Generic;
using System.IO;
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
            Opciones? seleccion = oposMenu.CargarMenu();

            switch (seleccion)
            {
                case Opciones.Cargar:
                    string ruta = SolicitarRutaArchivo();
                    if (!string.IsNullOrEmpty(ruta))
                    {
                        poolPreguntas = await miImportador.CargarDesdeTexto(ruta);
                    }
                    Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                    Console.ReadKey();
                    break;

                case Opciones.Comenzar:
                    if (poolPreguntas.Count > 0)
                    {
                        Examen nuevoExamen = new(poolPreguntas);
                        Test testActual = new(nuevoExamen);
                        if (testActual.Temas != null && testActual.Temas?.Count > 1)
                        {
                            List<string> opcionesMenu = new List<string> { "TODOS LOS TEMAS" };
                            opcionesMenu.AddRange(testActual.Temas);
                            Console.WriteLine("Se han detectado varios Temas. ¿Qué quieres hacer?");
                            string seleccionUsuario = UIHelper.MostrarOpciones(opcionesMenu);

                            if (seleccionUsuario != "TODOS LOS TEMAS")
                            {
                                testActual.Filtrar(seleccionUsuario);
                            }
                        }
                        Console.Clear();
                        Console.WriteLine("Selecciona el sistema de puntuación:");
                        string penalizacionElegida = UIHelper.MostrarOpciones(testActual.opcionesPenalizacion);

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

                case Opciones.Instrucciones:
                    MostrarInstrucciones();
                    Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
                    Console.ReadKey();
                    break;

                case Opciones.Salir:
                    buclePrincipal = false;
                    break;
            }
        }
    }

    private static string SolicitarRutaArchivo()
    {
        Console.Clear();
        Console.WriteLine("Introduce la ruta del archivo de preguntas (.txt)");
        Console.WriteLine("(Puedes pegar la ruta o arrastrar el archivo aquí)\n");
        Console.Write("Ruta por defecto: ");

        string rutaPorDefecto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "preguntas.txt");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(rutaPorDefecto);
        Console.ResetColor();
        Console.WriteLine("\nPulsa Enter para usar la ruta por defecto, o escribe la ruta:");
        Console.Write("Ruta> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.CursorVisible = true;

        string? entrada = Console.ReadLine()?.Trim();
        Console.ResetColor();
        return string.IsNullOrWhiteSpace(entrada) ? rutaPorDefecto : entrada.Trim('"');
    }

    private static void MostrarInstrucciones()
    {
        Console.Clear();
        string? instrucciones = File.ReadAllText("instrucciones.txt");
        Console.WriteLine(instrucciones);
    }
}
