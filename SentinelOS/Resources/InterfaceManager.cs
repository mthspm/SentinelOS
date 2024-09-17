using Cosmos.System.Graphics;
using SentinelOS.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MouseManager = Cosmos.System.MouseManager;

namespace SentinelOS.Resources
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
            userInterface.Run();
            canvas.Display();
        }

    }
}
