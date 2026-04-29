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
        MenuInicio oposMenu = new();
        Importador miImportador = new();
        GestorBD bd = new();
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
                        List<string> modoExamen = new List<string> { "Examen normal", "Repaso de fallos" };
                        string modoElegido = UIHelper.MostrarOpciones(modoExamen);

                        Test testActual;

                        if (modoElegido == "Repaso de fallos")
                        {
                            Test? testRepaso = PrepararRepaso(poolPreguntas, bd);
                            if (testRepaso == null) break;
                            testActual = testRepaso;
                        }
                        else
                        {
                            Examen nuevoExamen = new(poolPreguntas);
                            testActual = new Test(nuevoExamen, bd);

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
                        }

                        Console.Clear();
                        List<string> opcionesAleatorio = new List<string> {
                            "Sin aleatorizar",
                            "Aleatorizar preguntas",
                            "Aleatorizar opciones de respuesta",
                            "Aleatorizar preguntas y opciones"
                        };
                        Console.WriteLine("¿Cómo quieres que se presenten las preguntas?");
                        string aleatorioElegido = UIHelper.MostrarOpciones(opcionesAleatorio);

                        testActual.AleatorizarPreguntas = aleatorioElegido.Contains("preguntas");
                        testActual.AleatorizarOpciones = aleatorioElegido.Contains("opciones");

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

                case Opciones.Estadisticas:
                    EstadisticasView.Mostrar(bd);
                    Console.WriteLine($"\n\n[Opos] Pulsa cualquier tecla para continuar");
                    Console.ReadKey();
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

    private static Test? PrepararRepaso(List<Pregunta> pool, GestorBD bd)
    {
        int totalFallos = bd.TotalFallosUnicos();
        if (totalFallos == 0)
        {
            Console.WriteLine("\nNo hay preguntas falladas registradas todavía.");
            Console.WriteLine("¡Realiza algún examen primero!");
            Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
            Console.ReadKey();
            return null;
        }

        List<FalloUnico> fallosUnicos = bd.ObtenerFallosUnicos();
        List<Pregunta> preguntasRepaso = new();

        foreach (var fallo in fallosUnicos)
        {
            Pregunta? coincidencia = pool.FirstOrDefault(p =>
                p.Enunciado == fallo.Enunciado && p.Tema == fallo.Tema);

            if (coincidencia != null)
            {
                Pregunta copia = new()
                {
                    Tema = coincidencia.Tema,
                    NombreTema = coincidencia.NombreTema,
                    NumeroPregunta = coincidencia.NumeroPregunta,
                    Enunciado = coincidencia.Enunciado,
                    Opciones = new List<string>(coincidencia.Opciones),
                    RespuestaCorrecta = fallo.RespuestaCorrecta
                };
                preguntasRepaso.Add(copia);
            }
        }

        if (preguntasRepaso.Count == 0)
        {
            Console.WriteLine($"\nSe encontraron {totalFallos} fallos, pero no coinciden con las preguntas cargadas.");
            Console.WriteLine("Carga el archivo de preguntas original para poder repasar.");
            Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
            Console.ReadKey();
            return null;
        }

        Console.WriteLine($"\nSe encontraron {preguntasRepaso.Count} preguntas para repasar de {totalFallos} fallos registrados.");
        Console.WriteLine($"\n[Opos] Pulsa cualquier tecla para continuar");
        Console.ReadKey();

        return new Test(preguntasRepaso, bd);
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
