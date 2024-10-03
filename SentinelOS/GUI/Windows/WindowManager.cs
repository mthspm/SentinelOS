using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI.Windows
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
                switch (window.GetWindowState())
                {
                    case WindowState.Running:
                        window.Run();
                        break;
                    case WindowState.ToClose:
                        RemoveWindow(window);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool HasActiveWindow()
        {
            return windows.Any(window => window.GetWindowState() == WindowState.Running);
        }

        public static bool HasHoveredWindow()
        {
            return windows.Any(window => window.IsHovered());
        }
    }
}
