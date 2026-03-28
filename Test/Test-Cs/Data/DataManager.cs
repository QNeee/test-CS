namespace Test_Cs.Data
{

    public interface IDataManagerInstance
    {
        public void Execute(Post item, string value, List<ResponseObj> list, string filterKey);
    }
    public class Post
    {
        public string title { get; set; } = "";
        public string text { get; set; } = "";
        public string url { get; set; } = "";
        public string kind { get; set; } = "";
        public string? this[string key]
        {
            get
            {
                return key.ToLower() switch
                {
                    "title" => title,
                    "text" => text,
                    _ => null
                };
            }
        }
    }
    public class DataManager
    {
        readonly string instancesPaths = "Test_Cs.Data.";
        readonly IDataManagerInstance _instance;
        readonly List<Post> _data = new List<Post>();
        readonly string manageKey = "";
        public DataManager(string flag, List<Post> data)
        {
            string className = Helper.MakeClassName(flag);
            string classNamePath = $"{instancesPaths}{className}";
            Type type = Helper.MakeType(classNamePath) ?? typeof(Filter);
            _instance = (IDataManagerInstance)Activator.CreateInstance(type)!;
            _data = data;
            manageKey = flag;
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
                        _instance.Execute(post, value, list, manageKey);
                    }
                }
            }
            return list;
        }

    }
}
