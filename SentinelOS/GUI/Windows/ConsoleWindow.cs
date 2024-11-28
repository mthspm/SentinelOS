using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources;
using SentinelOS.Resources.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        private void Help()
        {
            WriteLine("Commands:");
            WriteLine("clear - Clear the console");
            WriteLine("exit - Close the console");
            WriteLine("help - Display this help message");
        }

        private void ExecuteCommand(string command)
        {
            WriteLine("> " + command);
            if (command == "clear")
            {
                Clear();
            }
            else if (command == "exit")
            {
                windowState = WindowState.ToClose;
            }
            else if (command == "help")
            {
                Help();
            }
            else
            {
                try
                {
                    Lexer lexer = new Lexer(command);
                    List<Token> tokens = lexer.Tokenize();
                    Evaluator evaluator = new Evaluator(tokens);
                    double result = evaluator.Expression();
                    WriteLine(result.ToString());
                }
                catch (Exception ex)
                {
                    WriteLine("Error: " + ex.Message);
                }
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