
using System.Collections;
using System.Text.Json;
using Test_Cs.Data;

namespace Test_Cs
{
    struct Image
    {
        public string url { get; set; }
    }
    struct ImageData
    {
        public Image source { get; set; }
    }
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

                    string imageUrl = "";
                    string title = data.GetProperty("title").GetString() ?? "";
                    string text = data.GetProperty("selftext").GetString() ?? "";

                    if (data.TryGetProperty("preview", out JsonElement preview) &&
                        preview.TryGetProperty("images", out JsonElement images) &&
                        images.ValueKind == JsonValueKind.Array &&
                        images.GetArrayLength() > 0)
                    {
                        var firstImage = images[0];

                        if (firstImage.TryGetProperty("source", out JsonElement source) &&
                            source.TryGetProperty("url", out JsonElement urlElement))
                        {
                            imageUrl = urlElement.GetString() ?? "";
                        }
                    }

                    Post post = new Post
                    {
                        title = title,
                        text = text,
                        url = imageUrl
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
