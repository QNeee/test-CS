using Test_Cs.Data;

namespace Test_Cs.Requests.POST
{
    public class Search : IRequest
    {
        private readonly string defaultFilterValue = "title";
        private readonly int InitialLimit = 15;
        public async Task<Response> Execute(RequestData? data)
        {
            var result = new Response();

            if (data != null)
            {
                var tasks = data.items.Select(async item =>
                {
                    var posts = await RedditParser.GetPosts(item.subreddit, data.limit == 0 ? InitialLimit : data.limit);
                    var dataManager = new DataManager(posts);
                    var filtered = dataManager.FilterItems(item.keywords, data.filterBy ?? defaultFilterValue);
                    result.Data["/" + item.subreddit] = filtered;
                });
                await Task.WhenAll(tasks);
            }

            return result;
        }
    }
}
