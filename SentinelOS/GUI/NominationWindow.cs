using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources;
using System;
using System.Drawing;

namespace SentinelOS.GUI
{
    public class NominationWindow : Window
    {
        private string fileName;
        private Action<string> onNameEntered;

        public NominationWindow(Canvas canvas, int x, int y, int width, int height, string name, string filename, Action<string> onNameEntered)
            : base(canvas, x, y, width, height, name)
        {
            this.fileName = filename;
            this.onNameEntered = onNameEntered;
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
        }

        public override void Initialize(string path)
        {
            // Not needed for this window
        }

        public override void HandleKeyPress()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        if (fileName.Length > 0)
                        {
                            onNameEntered?.Invoke(fileName);
                        }
                        windowState = WindowState.ToClose;
                        break;
                    case ConsoleKey.Escape:
                        windowState = WindowState.ToClose;
                        break;
                    case ConsoleKey.Backspace:
                        if (fileName.Length > 0)
                        {
                            fileName = fileName.Substring(0, fileName.Length - 1);
                        }
                        break;
                    default:
                        if (keyInfo.KeyChar != '\0')
                        {
                            fileName += keyInfo.KeyChar;
                        }
                        break;
                }
            }
        }

        public override void HandleMouseInput()
        {
            // Not implemented yet
        }

        public override void Draw()
        {
            int windowX = (canvas.Mode.Columns - windowWidth) / 2;
            int windowY = (canvas.Mode.Rows - windowHeight) / 2;

            canvas.DrawFilledRectangle(new Pen(Color.White), windowX, windowY, windowWidth, windowHeight);

            canvas.DrawRectangle(new Pen(Color.Black), windowX, windowY, windowWidth, windowHeight);

            canvas.DrawString("Nome do Arquivo/Pasta:", PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 10);

            canvas.DrawFilledRectangle(new Pen(Color.White), windowX + 10, windowY + 40, windowWidth - 20, 20);
            canvas.DrawString(fileName, PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 40);
        }

        public override void Run()
        {
            Draw();
            HandleKeyPress();
        }
    }
}