

namespace Test_Cs.Data.Filter
{
    internal class Title : IFilter
    {
        public void Execute(Post item, string value, List<ResponseObj> list)
        {
            bool isMatch = item.title.Contains(value, StringComparison.OrdinalIgnoreCase);
            if (isMatch)
            {
                var obj = new ResponseObj(item.title, String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
        }
    }
}
