using Test_Cs.Data;

namespace Test_Cs.Requests.POST
{
    public class Search : IRequest
    {
        private readonly string defaultFilterValue = "title";
        private readonly int InitialLimit = 15;
        public Response Execute(RequestData? data)
        {
            var result = new Response();
            if (data != null)
            {
                foreach (var item in data.items)
                {
                    var dataBase = Context.GetListByKey(item.subreddit);
                    var dataManager = new DataManager(data?.filterBy ?? defaultFilterValue, dataBase);
                    result.Data["/" + item.subreddit] = dataManager.FilterItems(item.keywords, data?.limit ?? InitialLimit);
                }
            }
            return result;
        }
    }
}
