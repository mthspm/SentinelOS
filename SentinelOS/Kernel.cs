using Cosmos.Core.Memory;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using SentinelOS.GUI;
using SentinelOS.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Sys = Cosmos.System;

namespace SentinelOS
{
    public class Kernel : Sys.Kernel
    {
        private ConsoleManager consoleManager;

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("SentinelOS is booting...");
            DirectoryManager.CreateSystemFiles();
            consoleManager = new ConsoleManager();
            Console.WriteLine("SentinelOS has booted successfully!");
            Console.WriteLine("Type 'help' for a list of commands.");
        }

        protected override void Run()
        {
            string input = Console.ReadLine();
            consoleManager.ExecuteCommand(input);
        }
    }
}