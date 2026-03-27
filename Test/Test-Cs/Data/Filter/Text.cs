

namespace Test_Cs.Data.Filter
{
    internal class Text : Filter 
    {
        public override void Execute(Post item, string value, List<ResponseObj> list)
        {
            bool isMatch = item.text.Contains(value, StringComparison.OrdinalIgnoreCase);
            if (isMatch && _addedTexts.Add(item.text))
            {
                var obj = new ResponseObj(item.text, !String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
        }
    }
}
