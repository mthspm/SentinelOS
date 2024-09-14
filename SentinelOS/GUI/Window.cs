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
    public abstract class Window
    {
        protected Canvas canvas;
        protected int windowX;
        protected int windowY;
        protected int windowWidth;
        protected int windowHeight;
        protected bool isMaximized;
        protected bool isMinimized;
        protected bool isRunning;

        private int originalX, originalY, originalWidth, originalHeight;
        private bool isDragging = false;
        private int dragOffsetX;
        private int dragOffsetY;

        public string name { get; protected set; }

        private Rectangle minimizeButton;
        private Rectangle maximizeButton;
        private Rectangle closeButton;
        

        public Window(Canvas canvas, int x, int y, int width, int height, string name)
        {
            this.canvas = canvas;
            this.windowX = x;
            this.windowY = y;
            this.windowWidth = width;
            this.windowHeight = height;
            this.isRunning = false;
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
        public abstract void HandleKeyPress(ConsoleKeyInfo keyInfo);
        /// <summary>
        /// This method must call the <see cref="HandleEssentialMouseInput"/> in order to handle the essential mouse input for the window.
        /// </summary>
        public abstract void HandleMouseInput();
        public abstract void Draw();
        public bool IsRunning => isRunning;

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
                isRunning = false;
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

        //TODO: FIX MINIMIZE AND MAXIMIZE METHODS

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

                windowX = 0;
                windowY = 0;
                windowWidth = canvas.Mode.Columns;
                windowHeight = canvas.Mode.Rows;
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

            // Atualizar a posição dos botões
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