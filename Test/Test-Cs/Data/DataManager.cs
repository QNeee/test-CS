namespace Test_Cs.Data
{

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
        private readonly HashSet<string> _addedItems = new(StringComparer.OrdinalIgnoreCase);
        readonly List<Post> _data = new List<Post>();
        public DataManager(List<Post> data)
        {
            _data = data;
        }
        public List<ResponseObj> FilterItems(List<string> values,string filterBy)
        {
            var list = new List<ResponseObj>();
            if (_data != null)
            {
                foreach (var value in values)
                {
                    foreach (var post in _data)
                    {
                        string itemvalue = post[filterBy] ?? "";
                        bool isMatch = itemvalue.Contains(value, StringComparison.OrdinalIgnoreCase);
                        if (isMatch && _addedItems.Add(itemvalue))
                        {
                            var obj = new ResponseObj(itemvalue, !String.IsNullOrEmpty(post.url));
                            list.Add(obj);
                        }
                    }
                }
            }
            return list;
        }

    }
}
