using Cosmos.Core.Memory;
using SentinelOS.Resources;
using System;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;

namespace SentinelOS
{
    public class Kernel : Sys.Kernel
    {
        private ConsoleManager consoleManager;
        private InterfaceManager interfaceManager;

        protected override void BeforeRun()
        {
            Console.WriteLine("SentinelOS is booting...");
            DirectoryManager.CreateSystemFiles();
            //DirectoryManager.ClearDir(true);
            consoleManager = new ConsoleManager();

            Console.WriteLine("SentinelOS has booted successfully!");
            Console.Write("Press any key to continue...");
            Console.ReadKey();

            interfaceManager = new InterfaceManager(1280, 720);
            TaskManager.AddTask("InterfaceManager", 0, new Action(interfaceManager.Run));
        }

        protected override void Run()
        {
            //var input = Console.ReadLine();
            //consoleManager.ExecuteCommand(input);
            TaskManager.ExecuteAllTasks();
            Heap.Collect();
        }
    }
}