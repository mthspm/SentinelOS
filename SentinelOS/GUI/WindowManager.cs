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

        public void RemoveWindow(Window window) {
            windows.Remove(window);
        }

        public void Clear() {
            windows.Clear();
        }

        public List<Window> GetWindows() {
            return windows;
        }

        public void Run()
        {
            foreach (Window window in windows)
            {
                if (window.GetWindowState() == WindowState.Running)
                {
                    window.Run();
                }
            }
        }
    }
}
