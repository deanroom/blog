using System;
using System.Threading;

namespace ThreadsInDotNet
{
    public class Common
    {
        public static void WriteThreadInfo(string scope)
        {
            var thread = Thread.CurrentThread;
            Console.WriteLine($"Scope:{scope},{thread.Name},{thread.ManagedThreadId},{thread.IsBackground},{thread.IsThreadPoolThread}");
        }
    }
}