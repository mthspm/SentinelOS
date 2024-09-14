using Cosmos.System.Graphics;
using System;
using System.Drawing;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using Cosmos.System.FileSystem.VFS;
using System.Collections.Generic;
using System.Linq;
using Cosmos.Core;

namespace SentinelOS.GUI
{
    public class Notepad : Window
    {
        private List<string> textContent;
        private int cursorX;
        private int cursorY;
        private string filePath;

        public Notepad(Canvas canvas, int x, int y, int width, int height)
            : base(canvas, x, y, width, height)
        {
            this.textContent = new List<string> { string.Empty };
            this.cursorX = 0;
            this.cursorY = 0;
            this.filePath = string.Empty;
        }

        public override void Initialize()
        {
            isRunning = true;
        }

        public override void Initialize(string path)
        {
            try
            {
                if (VFSManager.FileExists(path))
                {
                    textContent = File.ReadAllLines(path).ToList();
                }
                else
                {
                    textContent = new List<string> { string.Empty };
                    File.WriteAllLines(path, textContent);
                }
                filePath = path;
                SetCursorToEnd();
                isRunning = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o Notepad: {ex.Message}");
                isRunning = false;
            }
        }

        private void SetCursorToEnd()
        {
            if (textContent.Count > 0)
            {
                cursorY = textContent.Count - 1;
                cursorX = textContent[cursorY].Length;
            }
            else
            {
                cursorX = 0;
                cursorY = 0;
                textContent.Add(string.Empty);
            }
        }

        public override void HandleKeyPress(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    string newLine = textContent[cursorY].Substring(cursorX);
                    textContent[cursorY] = textContent[cursorY].Substring(0, cursorX);
                    textContent.Insert(cursorY + 1, newLine);
                    cursorY++;
                    cursorX = 0;
                    break;
                case ConsoleKey.Backspace:
                    if (cursorX > 0)
                    {
                        textContent[cursorY] = textContent[cursorY].Remove(cursorX - 1, 1);
                        cursorX--;
                    }
                    else if (cursorY > 0)
                    {
                        string currentLine = textContent[cursorY];
                        cursorY--;
                        cursorX = textContent[cursorY].Length;
                        textContent[cursorY] += currentLine;
                        textContent.RemoveAt(cursorY + 1);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                    HandleCursorMovement(keyInfo);
                    break;
                case ConsoleKey.S when keyInfo.Modifiers == ConsoleModifiers.Control:
                    SaveFile();
                    break;
                case ConsoleKey.X when keyInfo.Modifiers == ConsoleModifiers.Control:
                case ConsoleKey.Escape:
                    isRunning = false;
                    break;
                default:
                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        textContent[cursorY] = textContent[cursorY].Insert(cursorX, keyInfo.KeyChar.ToString());
                        cursorX++;
                    }
                    break;
            }
        }

        private void HandleCursorMovement(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (cursorX > 0) cursorX--;
                    break;
                case ConsoleKey.RightArrow:
                    if (cursorX < textContent[cursorY].Length) cursorX++;
                    break;
                case ConsoleKey.UpArrow:
                    if (cursorY > 0)
                    {
                        cursorY--;
                        cursorX = Math.Min(textContent[cursorY].Length, cursorX);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (cursorY < textContent.Count - 1)
                    {
                        cursorY++;
                        cursorX = Math.Min(textContent[cursorY].Length, cursorX);
                    }
                    break;
            }
        }

        private void SaveFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (string line in textContent)
                    {
                        writer.WriteLine(line);
                    }
                }
                Console.WriteLine("\nFile saved with success at " + filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public override void Draw()
        {
            if (isRunning)
            {
                canvas.DrawFilledRectangle(new Pen(Color.White), windowX, windowY, windowWidth, windowHeight);
                canvas.DrawRectangle(new Pen(Color.Black), windowX, windowY, windowWidth, windowHeight);

                for (int i = 0; i < textContent.Count; i++)
                {
                    canvas.DrawString(textContent[i], PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 10 + i * 20);
                }

                DrawCursor();
                canvas.Display();
            }
        }

        private void DrawCursor()
        {
            int cursorPosX = windowX + 10 + cursorX * 8;
            int cursorPosY = windowY + 10 + cursorY * 20;
            canvas.DrawFilledRectangle(new Pen(Color.Black), cursorPosX, cursorPosY, 2, 16);
        }
    }
}
