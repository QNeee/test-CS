

using System.Text.Json;
using Test_Cs.Errors;
using System.Collections.Concurrent;
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
        public bool File = data?.file ?? false;
    }
    public struct ResponseObj
    {
        public string text { get; set; }
        public bool hasImage { get; set; }

        public ResponseObj(string itemText, bool itemHasImage)
        {
            text = itemText;
            hasImage = itemHasImage;
        }
    }
    public class Response
    {
        public ConcurrentDictionary<string, List<ResponseObj>> Data { get; set; } = new();
    }
    public class RequestData
    {
        public List<Item> items { get; set; } = new();
        public int limit { get; set; }
        public string filterBy { get; set; } = "";
        public bool file { get; set; }
    }

    public class Item
    {
        public string subreddit { get; set; } = "";
        public List<string> keywords { get; set; } = new();
    }
    public interface IRequest
    {
        Task<Response> Execute(RequestData? data);
    }
    internal class Context
    {
        readonly IRequest _instance;
        readonly RequestData? Data;
        readonly string routesMethodsPath = "Test_Cs.Requests.";
        readonly string ReqUrl = "";
        readonly string ReqMethod = "";
        readonly QueryParam[] _params;
        readonly Dictionary<string, HashSet<string>> _routes = new()
        {
            ["POST"] = new()
            {
                "search"
            }
        };
        public Context(RequestSettings req)
        {

            string reqUrl = req.Url.Trim('/').ToLower();
            bool isRouteExist = _routes.TryGetValue(req.Method, out var methodRoutes) && methodRoutes.Contains(reqUrl);
            if (!isRouteExist) throw new NotFoundException($"Маршрут '{reqUrl}' не знайдено!");
            string className = Helper.MakeClassName(reqUrl);
            string classNamePath = $"{routesMethodsPath}{req.Method}.{className}";
            Type type = Helper.MakeType(classNamePath) ?? throw new NotFoundException($"Обробник для методу '{req.Method}' по маршруту {reqUrl} не знайдено!");
            _instance = (IRequest)Activator.CreateInstance(type)!;
            Data = req.Data;
            ReqMethod = req.Method;
            ReqUrl = req.Url;
            _params =  req.Params;
        }
        public async Task<Response> Execute()
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
            var result = await _instance.Execute(Data);
            loger.Log(JsonSerializer.Serialize(result.Data, new JsonSerializerOptions { WriteIndented = true }));
            loger.Log("=========================================");
            return result;
        }
    }

}