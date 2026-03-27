

namespace Test_Cs.Data.Filter
{
    internal class Text : IFilter
    {
        public void Execute(Post item, string value, List<ResponseObj> list)
        {
            bool isMatch = item.text.Contains(value, StringComparison.OrdinalIgnoreCase);
            if (isMatch)
            {
                var obj = new ResponseObj(item.text, String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
        }
    }
}
