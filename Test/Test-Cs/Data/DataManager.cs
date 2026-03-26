
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
        readonly List<Post> _data = new List<Post>();
        public DataManager(string flag, List<Post> data)
        {
            string className = Context.MakeClassName(flag);
            Assembly asm = Assembly.GetExecutingAssembly();
            string classNamePath = $"{filtersPaths}{className}";
            Type type = asm.GetType(classNamePath) ?? typeof(Title);
            _filter = (IFilter)Activator.CreateInstance(type)!;
            _data = data;
        }
        public List<ResponseObj> FilterItems(List<string> values, int limit)
        {
            var list = new List<ResponseObj>();
            if (_data != null)
            {
                int count = 0;
                foreach (var value in values)
                {
                    for (int i = 0; i < _data.Count; i++)
                    {
                        if (count >= limit) break;
                        var item = _data[i];
                        if (_filter.Execute(item, value, list)) count++;
                    }
                }
            }
            return list;
        }

    }
}
