
using System.Reflection;
using System.Text.Json;
using Test_Cs.Data.Filter;

namespace Test_Cs.Data
{
    public interface IFilter
    {
        bool Execute(Post item, string value, List<ResponseObj> list);
    }
    public class Post
    {
        public string title { get; set; } = "";
        public string author { get; set; } = "";
        public string text { get; set; } = "";
        public int ups { get; set; }
        public int downs { get; set; }
        public int score { get; set; }
        public string id { get; set; } = "";
        public string permalink { get; set; } = "";
        public string url { get; set; } = "";
        public int num_comments { get; set; }
        public long created_utc { get; set; }
    }
    public class DataManager
    {
        readonly string filtersPaths = "Test_Cs.Data.Filter.";
        readonly IFilter _filter;
        public DataManager(string flag)
        {
            string className = Context.MakeClassName(flag);
            Assembly asm = Assembly.GetExecutingAssembly();
            string classNamePath = $"{filtersPaths}{className}";
            Type type = asm.GetType(classNamePath) ?? typeof(Title);
            _filter = (IFilter)Activator.CreateInstance(type)!;
        }
        public List<ResponseObj> FilterItems(List<string> values, string key, int limit)
        {
            var needItems = GetListByKey(key);
            var list = new List<ResponseObj>();
            if (needItems != null)
            {
                int count = 0;
                foreach (var value in values)
                {
                    for (int i = 0; i < needItems.Count; i++)
                    {
                        if (count >= limit) break;
                        var item = needItems[i];
                        if (_filter.Execute(item, value, list)) count++;
                    }
                }
            }
            return list;
        }

        private static List<Post>? GetListByKey(string key)
        {
            var dictionary = GetData();
            return dictionary.TryGetValue(key, out var dictKeywords) ? dictKeywords : null;
        }
        private static Dictionary<string, List<Post>> GetData()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, List<Post>>>(json);
            if (data?.Count > 0) return data;
            return new Dictionary<string, List<Post>>();
        }
    }
}
