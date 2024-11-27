﻿using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources;
using System;
using System.Drawing;

namespace SentinelOS.GUI.Windows
{
    public class NominationWindow : Window
    {
        private string fileName;
        private Action<string> createAction;
        private Func<string, string, bool> renameAction;
        private string oldName;
        private Action refresh;

        public NominationWindow(Canvas canvas, int x, int y, int width, int height, string name, string filename, Action<string> createAction, Action refresh)
            : base(canvas, x, y, width, height, name)
        {
            fileName = filename;
            this.createAction = createAction;
            this.refresh = refresh;
        }

        public NominationWindow(Canvas canvas, int x, int y, int width, int height, string name, string oldName, Func<string, string, bool> renameAction, Action refresh)
            : base(canvas, x, y, width, height, name)
        {
            this.oldName = oldName;
            fileName = oldName;
            this.renameAction = renameAction;
            this.refresh = refresh;
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
        }

        public override void Initialize(string path)
        {
            // Not needed for this window
        }

        public override void CheckWindowStateChanges()
        {
            // Not yet implemented
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
                            if (createAction != null)
                            {
                                createAction(fileName);
                                refresh();
                            }
                            else if (renameAction != null)
                            {
                                renameAction(oldName, fileName);
                                refresh();
                            }
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