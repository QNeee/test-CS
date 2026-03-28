namespace Test_Cs.Data
{
    internal class Filter : IDataManagerInstance
    {
        private readonly HashSet<string> _addedTexts = new(StringComparer.OrdinalIgnoreCase);
        public void Execute(Post item, string value, List<ResponseObj> list, string filterKey)
        {
            string itemvalue = item[filterKey] ?? "";
            bool isMatch = itemvalue.Contains(value, StringComparison.OrdinalIgnoreCase);
            if (isMatch && _addedTexts.Add(itemvalue))
            {
                var obj = new ResponseObj(itemvalue, !String.IsNullOrEmpty(item.url));
                list.Add(obj);
            }
        }
    }
}
