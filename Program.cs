using System.Threading.Tasks;

namespace Opos;

class Program
{
    static async Task Main(string[] args)
    {
        MenuInicio OposMenu = new();
        await OposMenu.CargarMenu();
    }
}
