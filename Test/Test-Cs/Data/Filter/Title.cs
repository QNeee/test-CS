

namespace Test_Cs.Data.Filter
{
    internal class Title : IFilter
    {
        public bool Execute(Post item, string value, List<ResponseObj> list)
        {
            bool isMatch = item.title.Contains(value, StringComparison.OrdinalIgnoreCase) && !list.Any(x => x.Text == item.title);
            if (isMatch)
            {
                var obj = new ResponseObj(item.title, String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
            return isMatch;
        }
    }
}
