using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using Cosmos.System.Graphics;
using SentinelOS.GUI;
using SentinelOS.GUI.Windows;
using SentinelOS.Resources.Handlers;
using MouseManager = Cosmos.System.MouseManager;

namespace SentinelOS.Resources.Managers
{
    class InterfaceManager
    {
        private Canvas canvas;
        private UserInterface userInterface;

        public InterfaceManager(int width, int height)
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(width, height, ColorDepth.ColorDepth32));
            SetupMouse(width, height);
            userInterface = new UserInterface(canvas);
            AlertHandler.Initialize(canvas);
        }

        private void SetupMouse(int width, int height)
        {
            MouseManager.ScreenWidth = (uint)width;
            MouseManager.ScreenHeight = (uint)height;
            MouseManager.X = (uint)width / 2;
            MouseManager.Y = (uint)height / 2;
        }

        public void Run()
        {
            userInterface.DrawUserInterface();
            if (!WindowManager.HasActiveWindow() && !WindowManager.HasHoveredWindow())
            {
                userInterface.HandleMouseInput();
            }
            WindowManager.Run();
            userInterface.DrawCursor((int)MouseManager.X, (int)MouseManager.Y);
            canvas.Display();
        }
    }
}
