using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI
{
    public abstract class Window
    {
        protected Canvas canvas;
        protected int windowX;
        protected int windowY;
        protected int windowWidth;
        protected int windowHeight;
        protected bool isRunning;

        public Window(Canvas canvas, int x, int y, int width, int height)
        {
            this.canvas = canvas;
            this.windowX = x;
            this.windowY = y;
            this.windowWidth = width;
            this.windowHeight = height;
            this.isRunning = false;
        }

        public abstract void Initialize();
        public abstract void Initialize(string path);
        public abstract void HandleKeyPress(ConsoleKeyInfo keyInfo);
        public abstract void Draw();

        public bool IsRunning => isRunning;

    }
}