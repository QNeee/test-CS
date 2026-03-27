
using Test_Cs.Data.Filter;

namespace Test_Cs.Data
{
    public interface IFilter
    {
        void Execute(Post item, string value, List<ResponseObj> list);
    }
    public class Post
    {
        public string title { get; set; } = "";
        public string text { get; set; } = "";
        public string url { get; set; } = "";
    }
    public class DataManager
    {
        readonly string filtersPaths = "Test_Cs.Data.Filter.";
        readonly IFilter _filter;
        readonly List<Post> _data = new List<Post>();
        public DataManager(string flag, List<Post> data)
        {
            string className = Helper.MakeClassName(flag);
            string classNamePath = $"{filtersPaths}{className}";
            Type type = Helper.MakeType(classNamePath) ?? typeof(Title);
            _filter = (IFilter)Activator.CreateInstance(type)!;
            _data = data;
        }
        public List<ResponseObj> FilterItems(List<string> values)
        {
            var list = new List<ResponseObj>();
            if (_data != null)
            {
                foreach (var value in values)
                {
                    foreach (var post in _data)
                    {
                        _filter.Execute(post, value, list);
                    }
                }
            }
            return list;
        }

    }
}
