

using System.Reflection;
using System.Text.Json;
using Test_Cs.Errors;
namespace Test_Cs
{
    public class QueryParam
    {
        public string Key { get; set; } = "";

        public string Value { get; set; } = "";
    }
    public struct RequestSettings(string method, string url, RequestData? data, QueryParam[] param)
    {
        public string Method = method;
        public string Url = url;
        public QueryParam[] Params = param;
        public RequestData? Data = data;
    }
    public struct ResponseObj
    {
        public string Text { get; set; }
        public bool HasImage { get; set; }

        public ResponseObj(string text, bool hasImage)
        {
            Text = text;
            HasImage = hasImage;
        }
    }
    public class Response
    {
        public Dictionary<string, List<ResponseObj>> Data { get; set; } = new();
    }
    public class RequestData
    {
        public List<Item> items { get; set; } = new();
        public int limit { get; set; }
        public string filterBy { get; set; } = "";
    }

    public class Item
    {
        public string subreddit { get; set; } = "";
        public List<string> keywords { get; set; } = new();
    }
    public interface IRequest
    {
        Response Execute(RequestData? data);
    }
    internal class Context
    {
        readonly string[] _routes = { "search" };
        readonly IRequest? _instance;
        readonly RequestData? Data;
        readonly string routesMethodsPath = "Test_Cs.Requests.";
        readonly string ReqUrl = "";
        readonly string ReqMethod = "";
        public Context(RequestSettings req)
        {

            string reqUrl = req.Url.Trim('/').ToLower();
            if (!_routes.Contains(reqUrl)) throw new NotFoundException($"Маршрут '{reqUrl}' не знайдено!");
            string className = MakeClassName(reqUrl);
            string classNamePath = $"{routesMethodsPath}{req.Method}.{className}";
            Assembly asm = Assembly.GetExecutingAssembly();
            Type type = asm.GetType(classNamePath) ?? throw new NotFoundException($"Обробник для методу '{req.Method}' по маршруту {reqUrl} не знайдено!");
            _instance = (IRequest)Activator.CreateInstance(type)!;
            Data = req.Data;
            ReqMethod = req.Method;
            ReqUrl = req.Url;
        }
        public static string MakeClassName(string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1);
        }
        public Response Execute()
        {
            var loger = Logger.GetInstance();
            loger.Log("=========================================");
            loger.Log("request");
            loger.Log(ReqMethod + ":" + ReqUrl);
            if (Data != null)
            {
                loger.Log("body");
                loger.Log(JsonSerializer.Serialize(Data.items, new JsonSerializerOptions { WriteIndented = true }));
            }

            loger.Log("response");
            var result = _instance?.Execute(Data)!;
            loger.Log(JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true }));
            loger.Log("=========================================");
            return result;
        }
    }

}