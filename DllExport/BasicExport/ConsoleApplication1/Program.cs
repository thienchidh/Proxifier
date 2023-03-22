using System.Threading;
using Proxifier;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Launcher.doStart(12344);
            // sleep for 10 seconds
            Thread.Sleep(1000000);
            Launcher.doStop();
        }
    }
}