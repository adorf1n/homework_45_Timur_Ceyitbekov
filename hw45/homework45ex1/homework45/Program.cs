using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string currentDir = Directory.GetCurrentDirectory();
        string site = currentDir + @"\site";

        DumbHttpServer server = new DumbHttpServer();
        await server.RunAsync(site, 8000);
    }
}
