using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI
{
    public class WindowManager
    {
        private Canvas canvas;
        private List<Window> windows;

        public WindowManager(Canvas canvas)
        {
            this.canvas = canvas;
            this.windows = new List<Window>();
        }

        public void AddWindow(Window window)
        {
            windows.Add(window);
        }

        public void Run()
        {
            foreach (var window in windows)
            {
                if (window.IsRunning)
                {
                    window.Draw();
                    window.HandleMouseInput();
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey(true);
                        window.HandleKeyPress(keyInfo);
                    }
                }
            }
        }
    }
}
