

using System.Text.Json;
using Test_Cs.Data;
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
            string className = Helper.MakeClassName(reqUrl);
            string classNamePath = $"{routesMethodsPath}{req.Method}.{className}";
            Type type = Helper.MakeType(classNamePath) ?? throw new NotFoundException($"Обробник для методу '{req.Method}' по маршруту {reqUrl} не знайдено!");
            _instance = (IRequest)Activator.CreateInstance(type)!;
            Data = req.Data;
            ReqMethod = req.Method;
            ReqUrl = req.Url;
        }
        private static Dictionary<string, List<Post>> GetDatabseData()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, List<Post>>>(json);
            if (data?.Count > 0) return data;
            return new Dictionary<string, List<Post>>();
        }
        public static List<Post> GetListByKey(string key)
        {
            return GetDatabseData().TryGetValue(key, out var dictKeywords) ? dictKeywords : new List<Post>();
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