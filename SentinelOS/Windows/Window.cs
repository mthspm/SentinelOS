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
using Point = Cosmos.System.Graphics.Point;

namespace SentinelOS.Windows
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
        protected bool isDragging = false;
        protected WindowState windowState;

        private int originalX, originalY, originalWidth, originalHeight;
        private int dragOffsetX;
        private int dragOffsetY;
        
        private Rectangle minimizeButton;
        private Rectangle maximizeButton;
        
        private Rectangle closeButton;
        private const int MarginY = 30;
        private const int MarginX = 10;

        public string name { get; protected set; }

        public abstract void Initialize();
        public abstract void Initialize(string path);
        /// <summary>
        /// This method can be used to update icons or other elements of the window that may change over time.
        /// </summary>
        public abstract void CheckWindowStateChanges();
        public abstract void HandleKeyPress();
        public abstract void HandleMouseInput();
        public abstract void Draw();
        public abstract void Run();

        public Window(Canvas canvas, int x, int y, int width, int height, string name)
        {
            this.canvas = canvas;
            windowX = x;
            windowY = y;
            windowWidth = width;
            windowHeight = height;
            windowState = WindowState.Sleeping;
            this.name = name;

            originalX = x;
            originalY = y;
            originalWidth = width;
            originalHeight = height;

            int buttonWidth = 20;
            int buttonHeight = 20;
            int buttonPadding = 5;

            closeButton = new Rectangle(windowX + windowWidth - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
            maximizeButton = new Rectangle(closeButton.X - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
            minimizeButton = new Rectangle(maximizeButton.X - buttonWidth - buttonPadding, windowY - 25 + buttonPadding, buttonWidth, buttonHeight);
        }

        public WindowState GetWindowState()
        {
            return windowState;
        }

        /// <summary>
        /// This method should be called in the <see cref="Draw"/> overried method of every new class that inherits from <see cref="Window"/>.
        /// </summary>
        protected void DrawTitleBar(Color color)
        {
            canvas.DrawFilledRectangle(new Pen(color), windowX, windowY - 25, windowWidth, 30);
            canvas.DrawString(name, PCScreenFont.Default, new Pen(Color.Black), windowX + 5, windowY - 20);
            canvas.DrawFilledRectangle(new Pen(Color.Black), windowX, windowY + 5, windowWidth, 2);

            DrawButton(closeButton, Color.Red, Color.Black, "X");
            DrawButton(maximizeButton, Color.Black, Color.White, isMaximized ? "❐" : "□");
            DrawButton(minimizeButton, Color.Black, Color.White, "_");
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

        protected void DrawButton(Rectangle buttonRect, Color backgroundColor, Color textColor, string text)
        {
            canvas.DrawFilledRectangle(new Pen(backgroundColor), buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height);
            canvas.DrawRectangle(new Pen(Color.Black), buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height);
            canvas.DrawString(text, PCScreenFont.Default, new Pen(textColor), buttonRect.X + 5, buttonRect.Y + 5);
        }

        protected List<string> WrapTextToFitWindow(string text)
        {
            List<string> lines = new List<string>();
            StringBuilder currentLine = new StringBuilder();
            string[] words = text.Split(' ');

            // Assumindo uma fonte monoespaçada, definimos um número máximo de caracteres por linha
            int maxCharsPerLine = windowWidth / 8; // Ajuste o divisor conforme necessário

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 <= maxCharsPerLine)
                {
                    if (currentLine.Length > 0)
                    {
                        currentLine.Append(" ");
                    }
                    currentLine.Append(word);
                }
                else
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }

            return lines;
        }

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
            UpdateButtonsPositions();
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
            UpdateButtonsPositions();
        }

        private void DragWindow()
        {
            windowX = (int)MouseManager.X - dragOffsetX;
            windowY = (int)MouseManager.Y - dragOffsetY;

            ClampToScreenBounds();
            UpdateButtonsPositions();
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
        }

        private void UpdateButtonsPositions()
        {
            closeButton.X = windowX + windowWidth - closeButton.Width - 5;
            closeButton.Y = windowY - 25 + 5;
            maximizeButton.X = closeButton.X - maximizeButton.Width - 5;
            maximizeButton.Y = windowY - 25 + 5;
            minimizeButton.X = maximizeButton.X - minimizeButton.Width - 5;
            minimizeButton.Y = windowY - 25 + 5;
        }
    }
}