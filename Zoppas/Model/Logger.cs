namespace Zoppas.Model
{
    using log4net;

    public static class Logger
    {
        static Logger()
        {
            _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static ILog _Log;

        public static void Info(string message)
        {
            _Log.Info(message);
        }

        public static void Fatal(string message)
        {
            _Log.Fatal(message);
        }
    }
}
