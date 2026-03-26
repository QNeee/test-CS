

namespace Test_Cs.Data.Filter
{
    internal class Text : IFilter
    {
        public bool Execute(Post item, string value, List<ResponseObj> list)
        {
            bool isMatch = item.text.Contains(value, StringComparison.OrdinalIgnoreCase) && !list.Any(x => x.Text == item.text);
            if (isMatch)
            {
                var obj = new ResponseObj(item.text, String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
            return isMatch;
        }
    }
}
