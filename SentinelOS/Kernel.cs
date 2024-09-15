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
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem.Listing;

namespace SentinelOS
{
    public class Kernel : Sys.Kernel
    {
        private ConsoleManager consoleManager;
        private Canvas canvas;
        private UserInterface userInterface;

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("SentinelOS is booting...");

            DirectoryManager.CreateSystemFiles();
            DirectoryManager.CurrentPath = Config.Desktop;
            DirectoryManager.ClearDir();
            consoleManager = new ConsoleManager();

            MouseManager.ScreenWidth = 1280;
            MouseManager.ScreenHeight = 720;
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
            MouseManager.X = 1280 / 2;
            MouseManager.Y = 720 / 2;

            userInterface = new UserInterface(canvas);

            // Add tasks to the task manager
            // Example with a method that have arguments
            // taskManager.AddTask("ExampleTaskWithParameters", 0, new Action<int, string>(ExampleTaskWithParameters), 42, "Hello");
            TaskManager.AddTask("DrawUserInterface", 0, new Action(userInterface.DrawUserInterface));
            TaskManager.AddTask("HandleMouseInput", 0, new Action(userInterface.HandleMouseInput));

            Console.WriteLine("SentinelOS has booted successfully!");
            Console.WriteLine("Type 'help' for a list of commands.");
        }

        protected override void Run()
        {
            TaskManager.ExecuteAllTasks();
            Heap.Collect();
            canvas.Display();
        }
    }
}