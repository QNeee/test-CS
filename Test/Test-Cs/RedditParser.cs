
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Test_Cs.Data;

namespace Test_Cs
{
    internal class RedditParser
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<List<Post>> GetPosts(string subreddit, int limit = 5)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; TestApp/1.0)");

            string url = $"https://www.reddit.com/{subreddit}.json?limit={limit}";
            var posts = new List<Post>();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Запит не вдався: {(int)response.StatusCode} {response.ReasonPhrase}");
                    return posts;
                }

                string content = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(content);
                var childrens = doc.RootElement.GetProperty("data").GetProperty("children").EnumerateArray();
                foreach (var child in childrens)
                {
                    var data = child.GetProperty("data");
                    var title = data.GetProperty("title").GetString() ?? "";
                    var urlImage = data.GetProperty("url").GetString() ?? "";
                    var text = data.GetProperty("selftext").GetString() ?? "";
                    Post post = new Post
                    {
                        title = title,
                        text = text,
                        url = urlImage
                    };
                    posts.Add(post);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Помилка при запиті до Reddit: " + ex.Message);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Помилка парсингу JSON: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Інша помилка: " + ex.Message);
            }
            return posts;
        }
    }
}
