using Cosmos.System.Graphics;
using System;
using System.Drawing;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using Cosmos.System.FileSystem.VFS;

namespace SentinelOS.GUI
{
    public class Notepad
    {
        private Canvas canvas;
        private string textContent;
        private int cursorX;
        private int cursorY;
        private int windowX;
        private int windowY;
        private int windowWidth;
        private int windowHeight;
        private bool isRunning;
        private string filePath;

        public Notepad(Canvas canvas)
        {
            this.canvas = canvas;
            this.textContent = string.Empty;
            this.cursorX = 10;
            this.cursorY = 30;
            this.windowX = 100;
            this.windowY = 100;
            this.windowWidth = 600;
            this.windowHeight = 400;
            this.isRunning = false;
            this.filePath = string.Empty;
        }

        // TODO: add a option to create a new file if the path doesn't exist
        public void Initialize(string path) 
        {
            if (VFSManager.FileExists(path))
            {
                textContent = File.ReadAllText(path);
                filePath = path;
            }
            isRunning = true;
            DrawNotepadWindow();
        }

        public void HandleKeyPress(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    textContent += "\n";
                    cursorY += 20;
                    cursorX = 10;
                    break;
                case ConsoleKey.Backspace:
                    if (textContent.Length > 0)
                    {
                        textContent = textContent.Substring(0, textContent.Length - 1);
                        cursorX -= 10;
                        if (cursorX < 10)
                        {
                            cursorX = windowWidth - 20;
                            cursorY -= 20;
                        }
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    cursorX -= 10;
                    if (cursorX < 10)
                    {
                        cursorX = windowWidth - 20;
                        cursorY -= 20;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    cursorX += 10;
                    if (cursorX > windowWidth - 20)
                    {
                        cursorX = 10;
                        cursorY += 20;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    cursorY -= 20;
                    if (cursorY < 30)
                    {
                        cursorY = 30;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    cursorY += 20;
                    if (cursorY > windowHeight - 30)
                    {
                        cursorY = windowHeight - 30;
                    }
                    break;
                case ConsoleKey.S when keyInfo.Modifiers == ConsoleModifiers.Control:
                    SaveFile();
                    break;
                case ConsoleKey.X when keyInfo.Modifiers == ConsoleModifiers.Control:
                    isRunning = false;
                    break;
                case ConsoleKey.Escape:
                    isRunning = false;
                    break;
                default:
                    textContent += keyInfo.KeyChar;
                    cursorX += 10;
                    if (cursorX > windowWidth - 20)
                    {
                        cursorX = 10;
                        cursorY += 20;
                    }
                    break;
            }
        }

        private void SaveFile()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                File.WriteAllText(filePath, textContent);
            }
        }

        private void DrawNotepadWindow()
        {
            while (isRunning)
            {
                canvas.DrawFilledRectangle(new Pen(Color.White), windowX, windowY, windowWidth, windowHeight);
                canvas.DrawRectangle(new Pen(Color.Black), windowX, windowY, windowWidth, windowHeight);
                canvas.DrawString(textContent, PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 10);
                DrawCursor();
                canvas.Display();

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    HandleKeyPress(keyInfo);
                }
            }
        }

        private void DrawCursor()
        {
            canvas.DrawLine(new Pen(Color.Black), windowX + cursorX, windowY + cursorY, windowX + cursorX, windowY + cursorY + 20);
        }
    }
}
