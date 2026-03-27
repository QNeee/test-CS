
using System.Text.Json;
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

                foreach (var child in doc.RootElement.GetProperty("data").GetProperty("children").EnumerateArray())
                {
                    var data = child.GetProperty("data");
                    string title = data.GetProperty("title").GetString() ?? "";
                    string text = data.GetProperty("selftext").GetString() ?? "";
                    string itemUrl = data.GetProperty("url").GetString() ?? "";

                    posts.Add(new Post
                    {
                        title = title,
                        text = text,
                        url = itemUrl,
                    });
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
