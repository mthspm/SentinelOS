using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = Cosmos.System.Graphics.Point;
using Console = System.Console;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI.Windows
{
    public class AboutWindow : Window
    {
        private readonly List<string> aboutMessage = new List<string>
        {
            "SentinelOS - A Cosmos powered OS",
            "Version: 0.0.2",
            "GitHub: www.github.com/mthspm/SentinelOS",
            "Since 2024",
            "License: MIT",
        };
        private Pen aboutWindowColor;

        public AboutWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            return;
        }

        public override void Initialize()
        {
            aboutWindowColor = GetAboutWindowColor();
            windowState = WindowState.Running;
        }

        public override void Initialize(string path)
        {
            // Not needed for this window
        }

        public override void CheckWindowStateChanges()
        {
            if (isDragging || isMinimized || isMaximized)
            {
                // Not yet implemented
            }
        }

        public override void HandleKeyPress()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        windowState = WindowState.ToClose;
                        break;
                }
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput();
        }

        public override void Draw()
        {
            DrawAboutWindow();
            DrawTitleBar(Color.Gray);
        }

        private void DrawAboutWindow()
        {
            canvas.DrawFilledRectangle(aboutWindowColor, windowX, windowY, windowWidth, windowHeight);
            DrawAboutMessage();
        }


        private void DrawAboutMessage()
        {
            int messageY = windowY + 20;
            foreach (var message in aboutMessage)
            {
                canvas.DrawString(message, PCScreenFont.Default, new Pen(Color.Black), windowX + 5, messageY);
                messageY += 20;
            }
        }

        private Pen GetAboutWindowColor()
        {
            return new Pen(Color.LightGray);
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
        }
    }
}