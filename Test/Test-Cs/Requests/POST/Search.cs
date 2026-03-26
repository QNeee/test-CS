using System.ComponentModel;
using Test_Cs.Data;

namespace Test_Cs.Requests.POST
{
    public class Search : IRequest
    {
        private readonly string defaultFilterValue = "title";
        public Response Execute(RequestData? data)
        {

            var result = new Response();
            var dataManager = new DataManager(data?.filterBy ?? defaultFilterValue);
            if (data != null)
            {
                foreach (var item in data.items)
                {
                    var subreddit = item.subreddit;
                    var keyWords = item.keywords;
                    result.Data["/" +subreddit] = dataManager.FilterItems(keyWords, subreddit, data.limit);
                }
            }
            return result;
        }
    }
}
