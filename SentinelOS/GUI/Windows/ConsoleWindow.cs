using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SentinelOS.GUI.Windows
{
    public class ConsoleWindow : Window
    {
        private readonly List<string> outputLines = new List<string>();
        private string currentInput = string.Empty;

        public ConsoleWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            return;
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
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    ExecuteCommand(currentInput);
                    currentInput = string.Empty;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    currentInput += keyInfo.KeyChar;
                }
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput();
        }

        public override void Draw()
        {
            canvas.DrawFilledRectangle(new Pen(Color.DarkSlateGray), windowX, windowY, windowWidth, windowHeight);
            DrawTitleBar(Color.Gray);
            int lineY = windowY + 5;
            foreach (var line in outputLines)
            {
                canvas.DrawString(line, PCScreenFont.Default, new Pen(Color.White), windowX + 5, lineY);
                lineY += 20;
            }
            canvas.DrawString("> " + currentInput, PCScreenFont.Default, new Pen(Color.White), windowX + 5, lineY);
        }

        public void WriteLine(string line)
        {
            outputLines.Add(line);
            if (outputLines.Count > (windowHeight / 20) - 1)
            {
                outputLines.RemoveAt(0);
            }
        }

        private void Clear()
        {
            outputLines.Clear();
        }

        private void ExecuteCommand(string command)
        {
            WriteLine("> " + command);
            if (command.StartsWith("lua "))
            {
                string script = command.Substring(4);
                try
                {
                    WriteLine("Lua Not Supported Yet");
                    //luaInterpreter.Execute(script);
                }
                catch (Exception ex)
                {
                    WriteLine("Error: " + ex.Message);
                }
            }
            else if (command == "clear")
            {
                Clear();
            }
            else
            {
                WriteLine("Unknown command: " + command);
            }
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
        }
    }
}