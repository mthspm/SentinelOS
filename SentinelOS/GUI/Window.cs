using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;

namespace SentinelOS.GUI
{
    public enum WindowState
    {
        Sleeping,
        Running,
        ToClose
    }


    public abstract class Window
    {
        protected Canvas canvas;
        protected int windowX;
        protected int windowY;
        protected int windowWidth;
        protected int windowHeight;
        protected bool isMaximized;
        protected bool isMinimized;
        protected WindowState windowState;

        private int originalX, originalY, originalWidth, originalHeight;
        private bool isDragging = false;
        private int dragOffsetX;
        private int dragOffsetY;

        public string name { get; protected set; }

        private Rectangle minimizeButton;
        private Rectangle maximizeButton;
        private Rectangle closeButton;

        private const int MarginY = 30;
        private const int MarginX = 10;

        public Window(Canvas canvas, int x, int y, int width, int height, string name)
        {
            this.canvas = canvas;
            this.windowX = x;
            this.windowY = y;
            this.windowWidth = width;
            this.windowHeight = height;
            this.windowState = WindowState.Sleeping;
            this.name = name;

            this.originalX = x;
            this.originalY = y;
            this.originalWidth = width;
            this.originalHeight = height;

            int buttonWidth = 20;
            int buttonHeight = 20;
            int buttonPadding = 5;

            closeButton = new Rectangle(windowX + windowWidth - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
            maximizeButton = new Rectangle(closeButton.X - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
            minimizeButton = new Rectangle(maximizeButton.X - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
        }

        public abstract void Initialize();
        public abstract void Initialize(string path);
        public abstract void HandleKeyPress();
        /// <summary>
        /// This method must call <see cref="HandleEssentialMouseInput"/> in order to handle the essential mouse input for the window.
        /// </summary>
        public abstract void HandleMouseInput();
        public abstract void Draw();
        public abstract void Run();
        public WindowState GetWindowState()
        {
            return windowState;
        }

        protected void DrawTitleBar()
        {
            canvas.DrawFilledRectangle(new Pen(Color.Gray), windowX, windowY - 25, windowWidth, 30);
            canvas.DrawString(name, PCScreenFont.Default, new Pen(Color.Black), windowX + 5, windowY - 20);

            DrawButton(closeButton, Color.Red, "X");
            DrawButton(maximizeButton, Color.Black, isMaximized ? "❐" : "□");
            DrawButton(minimizeButton, Color.Black, "_");
        }
        /// <summary>
        /// This method should be called in the <see cref="HandleMouseInput"/> overried method of every new class that inherits from <see cref="Window"/>.
        /// It handles the essential mouse input for the window, such as closing the window, draging, minimizing it, etc.
        /// </summary>
        protected void HandleEssentialMouseInput()
        {
            if (MouseManager.X >= closeButton.X && MouseManager.X <= closeButton.X + closeButton.Width &&
                MouseManager.Y >= closeButton.Y && MouseManager.Y <= closeButton.Y + closeButton.Height &&
                MouseManager.MouseState == MouseState.Left)
            {
                windowState = WindowState.ToClose;
            }
            else if (MouseManager.X >= minimizeButton.X && MouseManager.X <= minimizeButton.X + minimizeButton.Width &&
                     MouseManager.Y >= minimizeButton.Y && MouseManager.Y <= minimizeButton.Y + minimizeButton.Height &&
                     MouseManager.MouseState == MouseState.Left)
            {
                Minimize();
            }
            else if (MouseManager.X >= maximizeButton.X && MouseManager.X <= maximizeButton.X + maximizeButton.Width &&
                     MouseManager.Y >= maximizeButton.Y && MouseManager.Y <= maximizeButton.Y + maximizeButton.Height &&
                     MouseManager.MouseState == MouseState.Left)
            {
                Maximize();
            }
            else if (MouseManager.X >= windowX && MouseManager.X <= windowX + windowWidth &&
                     MouseManager.Y >= windowY - 25 && MouseManager.Y <= windowY &&
                     MouseManager.MouseState == MouseState.Left)
            {
                if (!isDragging)
                {
                    isDragging = true;
                    dragOffsetX = (int)MouseManager.X - windowX;
                    dragOffsetY = (int)MouseManager.Y - windowY;
                }
            }
            else if (MouseManager.MouseState == MouseState.None)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                DragWindow();
            }
        }
        //TODO: FIX AND COMPLMENT IMPLEMENTATION -> MINIMIZE AND MAXIMIZE METHODS
        private void Minimize()
        {
            if (!isMinimized)
            {
                isMinimized = true;

                originalX = windowX;
                originalY = windowY;
                originalWidth = windowWidth;
                originalHeight = windowHeight;

                windowHeight = 30;
            }
            else
            {
                isMinimized = false;
                windowX = originalX;
                windowY = originalY;
                windowWidth = originalWidth;
                windowHeight = originalHeight;
            }
        }

        private void Maximize()
        {
            if (!isMaximized)
            {
                isMaximized = true;

                originalX = windowX;
                originalY = windowY;
                originalWidth = windowWidth;
                originalHeight = windowHeight;

                windowX = 50;
                windowY = 50;
                windowWidth = canvas.Mode.Columns - 200;
                windowHeight = canvas.Mode.Rows - 200;
            }
            else
            {
                isMaximized = false;
                windowX = originalX;
                windowY = originalY;
                windowWidth = originalWidth;
                windowHeight = originalHeight;
            }
        }

        private void DragWindow()
        {
            windowX = (int)MouseManager.X - dragOffsetX;
            windowY = (int)MouseManager.Y - dragOffsetY;

            ClampToScreenBounds();
        }

        private void ClampToScreenBounds()
        {
            if (windowX < MarginX)
            {
                windowX = MarginX;
            }
            else if (windowX + windowWidth > canvas.Mode.Columns - MarginX)
            {
                windowX = canvas.Mode.Columns - windowWidth - MarginX;
            }

            if (windowY < MarginY)
            {
                windowY = MarginY;
            }
            else if (windowY + windowHeight > canvas.Mode.Rows - MarginY)
            {
                windowY = canvas.Mode.Rows - windowHeight - MarginY;
            }

            closeButton.X = windowX + windowWidth - closeButton.Width - 5;
            closeButton.Y = windowY - 25 + 5;
            maximizeButton.X = closeButton.X - maximizeButton.Width - 5;
            maximizeButton.Y = windowY - 25 + 5;
            minimizeButton.X = maximizeButton.X - minimizeButton.Width - 5;
            minimizeButton.Y = windowY - 25 + 5;
        }

        private void DrawButton(Rectangle buttonRect, Color color, string text)
        {
            canvas.DrawFilledRectangle(new Pen(color), buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height);
            canvas.DrawString(text, PCScreenFont.Default, new Pen(Color.White), buttonRect.X + 5, buttonRect.Y + 5);
        }
    }
}