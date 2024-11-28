using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly Canvas canvas;
        private readonly UserInterface userInterface;
        private readonly LoginWindow loginWindow;

        public InterfaceManager(int width, int height)
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(width, height, ColorDepth.ColorDepth32));
            SetupMouse(width, height);
            userInterface = new UserInterface(canvas);
            loginWindow = new LoginWindow(canvas, 0, 0, width, height, "Login");
            loginWindow.Initialize();
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
            if (!HandleLoginWindow())
            {
                userInterface.DrawUserInterface();
                if (!WindowManager.HasActiveWindow() && !WindowManager.HasHoveredWindow())
                {
                    userInterface.HandleMouseInput();
                }

                WindowManager.Run();
            }
            userInterface.DrawCursor((int)MouseManager.X, (int)MouseManager.Y);
            canvas.Display();
        }

        private bool HandleLoginWindow()
        {
            if (loginWindow.GetWindowState() == WindowState.Running)
            {
                loginWindow.Run();
                return true;
            }
            return false;
        }
    }
}
