using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Windows
{
    public static class WindowManager
    {
        private static List<Window> windows;

        public static void Initialize()
        {
            windows = new List<Window>();
        }

        public static void AddWindow(Window window)
        {
            windows.Add(window);
        }

        public static void RemoveWindow(Window window)
        {
            windows.Remove(window);
        }

        public static void Clear()
        {
            windows.Clear();
        }

        public static List<Window> GetWindows()
        {
            return windows;
        }

        public static void Run()
        {
            foreach (Window window in windows)
            {
                if (window.GetWindowState() == WindowState.Running)
                {
                    window.Run();
                }
                else if (window.GetWindowState() == WindowState.ToClose)
                {
                    RemoveWindow(window);
                }
            }
        }
    }
}
