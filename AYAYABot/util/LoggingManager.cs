using System.Diagnostics;

namespace AYAYABot.util
{
    internal class LogManager
    {
        readonly string className;

        public LogManager(Type t)
        {
            this.className = t.Name;
        }

        public LogManager(string name)
        {
            this.className = name;
        }

        public void error(string message)
        {
            StackTrace st = new StackTrace();
            Console.WriteLine("[" + className + "." + st.GetFrame(1).GetMethod().ReflectedType.Name.Replace("d__2", "") + "] Error - " + message);
        }

        public void warn(string message)
        {
            StackTrace st = new StackTrace();
            Console.WriteLine("[" + className + "." + st.GetFrame(1).GetMethod().ReflectedType.Name.Replace("d__2", "") + "] Warn - " + message);
        }

        public void info(string message)
        {
            StackTrace st = new StackTrace();
            Console.WriteLine("[" + className + "." + st.GetFrame(1).GetMethod().ReflectedType.Name.Replace("d__2", "") + "] Info - " + message);
        }

        public void debug(string message)
        {
            StackTrace st = new StackTrace();
            Console.WriteLine("[" + className + "." + st.GetFrame(1).GetMethod().ReflectedType.Name.Replace("d__2", "") + "] Debug - " + message);
        }
    }
}