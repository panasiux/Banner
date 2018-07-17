using System;
using CommandDotNet;

namespace LoadTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var appRunner = new AppRunner<Commands>();
            appRunner.Run(args);
        }
    }
}
