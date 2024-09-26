using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        DumbHttpServer server = new DumbHttpServer();
        await server.RunAsync(8000); 
    }
}
