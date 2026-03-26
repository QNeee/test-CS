
namespace Test_Cs
{
    internal class Logger
    {
        private static Logger? _instance;
        private readonly string _logFile = "out.log";
        private Logger() { }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Logger();
            }
            return _instance;
        }

        public  void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            Console.WriteLine(logMessage);
            try
            {
                File.AppendAllText(_logFile, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося записати лог у файл: {ex.Message}");
            }
        }
    }
}