using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class DumbHttpServer
{
    private HttpListener _listener; 
    private int _port; 

    public async Task RunAsync(int port)
    {
        _port = port;

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
        _listener.Start();

        Console.WriteLine($"Сервер запущен на порту: {port}");

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
        string responseText;
        string contentType = "text/html; charset=utf-8";
        int statusCode;

        if (context.Request.HttpMethod == "GET")
        {
            switch (context.Request.Url.AbsolutePath)
            {
                case "/":
                    statusCode = (int)HttpStatusCode.OK;
                    responseText = "<h1>Follow the white rabbit</h1>";
                    break;
                case "/white_rabbit":
                    statusCode = (int)HttpStatusCode.OK;
                    responseText = "<h1>You are living in the matrix</h1>";
                    break;
                default:
                    statusCode = (int)HttpStatusCode.NotImplemented;
                    responseText = "<h1>501 Not Implemented</h1><p>The requested method or path is not implemented.</p>";
                    break;
            }
        }
        else
        {
            statusCode = (int)HttpStatusCode.NotImplemented;
            responseText = "<h1>501 Not Implemented</h1><p>The requested method is not implemented.</p>";
        }

        context.Response.ContentType = contentType;
        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(responseText);
        context.Response.StatusCode = statusCode;

        using (var output = context.Response.OutputStream)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            output.Write(buffer, 0, buffer.Length);
        }
    }
}
