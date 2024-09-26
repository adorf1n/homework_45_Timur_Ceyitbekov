using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class DumbHttpServer
{
    private string _siteDirectory; 
    private HttpListener _listener; 
    private int _port; 

    public async Task RunAsync(string path, int port)
    {
        _siteDirectory = path;
        _port = port;

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
        _listener.Start();

        Console.WriteLine($"Сервер запущен на порту: {port}");
        Console.WriteLine($"Файлы сайта лежат в папке: {path}");

        await ListenAsync();
    }

    public void Stop()
    {
        _listener.Abort();
        _listener.Stop();
    }

    private async Task ListenAsync()
    {
        try
        {
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                Process(context);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void Process(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath;
        Console.WriteLine($"Запрос: {path}");

        string filename = path.Substring(1);

        if (!Path.HasExtension(filename))
        {
            string responseStr = $"<h1>{filename}</h1>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);

            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
        }
        else
        {
            filename = Path.Combine(_siteDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream fileStream = new FileStream(filename, FileMode.Open);

                    context.Response.ContentType = GetContentType(filename);

                    context.Response.ContentLength64 = fileStream.Length;

                    byte[] buffer = new byte[16 * 1024]; 

                    int dataLength;
                    do
                    {
                        dataLength = fileStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, dataLength);
                    } while (dataLength > 0);

                    fileStream.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        context.Response.OutputStream.Close();
    }

 
    private static string GetContentType(string filename)
    {
        var dictionary = new Dictionary<string, string>
        {
            { ".css", "text/css" },
            { ".html", "text/html; charset=utf-8" }, 
            { ".ico", "image/x-icon" },
            { ".js", "application/x-javascript" },
            { ".json", "application/json" },
            { ".png", "image/png" }
        };

        string contentType = "";
        string fileExtension = Path.GetExtension(filename);
        dictionary.TryGetValue(fileExtension, out contentType);
        return contentType;
    }
}
