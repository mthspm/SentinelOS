using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using IL2CPU.API.Attribs;
using Cosmos.Core.Memory;
using MouseManager = Cosmos.System.MouseManager;
using Cosmos.Core;

namespace SentinelOS.GUI
{
    class UserInterface
    {
        Canvas canvas;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.cursor.bmp")] public static byte[] cursor;
        private readonly Bitmap cursorBmp = new Bitmap(cursor);

        public UserInterface()
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Aqua);
            MouseManager.ScreenWidth = 800;
            MouseManager.ScreenHeight = 600;
            MouseManager.X = MouseManager.ScreenWidth / 2;
            MouseManager.Y = MouseManager.ScreenHeight / 2;
        }

        public void StartGUI()
        {
            canvas.DrawRectangle(new Pen(Color.Black), 0, 580, 800, 20); // Taskbar

            //canvas.DrawImageAlpha(new Bitmap(SentinelOS.Resources.Resources.logo), 10, 10); // TODO: Add logo to dependencies.

            canvas.DrawTriangle(new Pen(Color.Red), 10, 10, 100, 100, 200, 10);

            canvas.DrawImageAlpha(cursorBmp, (int)MouseManager.X, (int)MouseManager.Y);

            Heap.Collect(); // Required to free memory

            canvas.Display(); // Required for something to be displayed when using a double buffered driver

            HandleKeyboardInput();
        }

        private void HandleKeyboardInput()
        {
            var pressedKey = Console.ReadKey().Key;
            if (pressedKey == ConsoleKey.Escape)
            {
                canvas.Disable();
            }
        }
    }
}
