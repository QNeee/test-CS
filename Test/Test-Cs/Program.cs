namespace Test_Cs
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = Environment.GetEnvironmentVariable("SERVER_URL") ?? "http://localhost:5000/";
            Server server = new Server(url);
            await server.Start();
        }
    }
}
