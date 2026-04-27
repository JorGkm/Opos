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
            // El método ahora devuelve la opción elegida
            Opciones seleccion = await oposMenu.CargarMenu();

            switch (seleccion)
            {
                case Opciones.Cargar:
                    string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "preguntas.txt");
                    poolPreguntas = await miImportador.CargarDesdeTexto(ruta);
                    Console.WriteLine($"\n[SISTEMA] Pulsa cualquier tecla para continuar");
                    Console.ReadKey();
                    break;

                case Opciones.Comenzar:
                    if (poolPreguntas.Count > 0)
                    {
                        Examen nuevoExamen = new Examen(poolPreguntas);
                        // Aquí llamarías a la lógica de hacer el examen
                    }
                    else
                    {
                        Console.WriteLine("\n[!] Primero debes cargar las preguntas.");
                        Console.ReadKey();
                    }
                    break;

                case Opciones.Salir:
                    buclePrincipal = false;
                    break;
            }
        }
    }
}
