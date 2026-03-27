using System.Net;
using System.Text;
using System.Text.Json;
using Test_Cs.Errors;

namespace Test_Cs
{
    internal class Server
    {
        private readonly HttpListener _listener;
        private bool _isRunning;

        public Server(string uriPrefix)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uriPrefix);
            _isRunning = false;
        }

        public async Task Start()
        {
            _listener.Start();
            _isRunning = true;
            Console.WriteLine("Сервер запущено....");

            while (_isRunning)
            {
                var httpContext = await _listener.GetContextAsync();
                _ = HandleRequest(httpContext);
            }
        }

        private static RequestSettings MakeSettings(string httpMethod, string route, string body, QueryParam[] param)
        {
            RequestData? requestData = null;

            if (!string.IsNullOrWhiteSpace(body))
            {
                requestData = JsonSerializer.Deserialize<RequestData>(body);
            }

            return new RequestSettings(httpMethod, route, requestData, param);
        }

        private static async Task WriteJsonResponse(HttpListenerContext httpContext, int statusCode, object data, bool isFile)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentEncoding = Encoding.UTF8;
            httpContext.Response.ContentLength64 = buffer.Length;
            httpContext.Response.ContentType = "application/json";

            if (isFile)
            {
                httpContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"result.json\"");
            }
            await httpContext.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            httpContext.Response.Close();
        }

        private static async Task HandleRequest(HttpListenerContext httpContext)
        {
            int statusCode = 200;
            object response= new object();
            bool isFile = false;
            try
            {
                string body = "";
                using (var reader = new StreamReader(httpContext.Request.InputStream, httpContext.Request.ContentEncoding))
                {
                    body = await reader.ReadToEndAsync();
                }
                QueryParam[] param = (httpContext.Request.QueryString.AllKeys ?? Array.Empty<string>())
    .Select(k => new QueryParam { Key = k ?? "", Value = httpContext.Request.QueryString[k] ?? "" })
    .ToArray();
                RequestSettings reqSett = MakeSettings(
                    httpContext.Request.HttpMethod ?? "",
                    httpContext.Request.Url?.AbsolutePath ?? "",
                    body,
                    param
                );

                Context context = new Context(reqSett);
                response = context.Execute().Result.Data;
                isFile = reqSett.File;
            }
            catch (Exception ex)
            {
                var notFound = ex as NotFoundException;
                statusCode = notFound?.StatusCode ?? 500;
                response = notFound != null ? notFound.Message : ex.Message;
                if (statusCode == 500)
                {
                    var logger = Logger.GetInstance();
                    logger.Log($"Error: {ex.Message}");
                }
            }

            await WriteJsonResponse(httpContext, statusCode, response, isFile);
        }
    }
}