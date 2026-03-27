
namespace Test_Cs.Data.Filter
{
    internal abstract class Filter : IFilter
    {
        protected readonly HashSet<string> _addedTexts = new(StringComparer.OrdinalIgnoreCase);
        public abstract void Execute(Post item, string value, List<ResponseObj> list);
    }
}
