using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources.CMD
{
    class Nano
    {
        public static void Main(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Please provide a file path.");
                return;
            }
            string filePath = DirectoryManager.CurrentPath + @"\" + path;
            if (DirectoryManager.IsValidSystemPath(filePath) && !Directory.Exists(filePath))
            {
                TextEditor editor = new TextEditor(filePath);
                editor.Run();
            }
        }

        class TextEditor
        {
            private string filePath;
            private List<string> lines;
            private int cursorX;
            private int cursorY;

            public TextEditor(string filePath)
            {
                this.filePath = filePath;
                lines = new List<string>();
                cursorX = 0;
                cursorY = 0;

                if (File.Exists(filePath))
                {
                    lines.AddRange(File.ReadAllLines(filePath));
                    LoadCursorPosition(lines);
                }
                else
                {
                    lines.Add(string.Empty);
                }
            }

            private void LoadCursorPosition(List<string> lines)
            {
                cursorY = lines.Count - 1;
                if (cursorY >= 0)
                {
                    cursorX = lines[cursorY].Length;
                }
                else
                {
                    cursorX = 0;
                    lines.Add(string.Empty);
                }
            }

            public void Run()
            {
                try
                {
                    ConsoleKeyInfo keyInfo;
                    do
                    {
                        LoadGraphics();
                        DisplayFile();
                        keyInfo = Console.ReadKey(true);
                        HandleKey(keyInfo);
                    } while (keyInfo.Key != ConsoleKey.Escape);

                    SaveFile();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Close();
                }
            }

            private void LoadGraphics()
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Clear();
                var commands = new string[] { "Esc: Save and Exit", "CTRL+A: Select All", "CTRL+C: Copy", "CTRL+V: Paste" };
                Console.SetCursorPosition(0, Console.WindowHeight - commands.Length);
                Console.WriteLine(string.Join(" | ", commands));
                Console.SetCursorPosition(0, 0);
            }

            private void Close()
            {
                Console.WriteLine("Press any key to exit nano...");
                Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
            }

            private void DisplayFile()
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    Console.WriteLine(lines[i]);
                }
                Console.SetCursorPosition(cursorX, cursorY);
            }

            private void HandleKey(ConsoleKeyInfo keyInfo)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursorY > 0)
                        {
                            cursorY--;
                            cursorX = Math.Min(lines[cursorY].Length, cursorX);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorY < lines.Count - 1)
                        {
                            cursorY++;
                            cursorX = Math.Min(lines[cursorY].Length, cursorX);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0) cursorX--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorX < lines[cursorY].Length) cursorX++;
                        break;
                    case ConsoleKey.Backspace:
                        if (cursorX > 0)
                        {
                            lines[cursorY] = lines[cursorY].Remove(cursorX - 1, 1);
                            cursorX--;
                        }
                        else if (cursorY > 0)
                        {
                            string currentLine = lines[cursorY];
                            cursorY--;
                            cursorX = lines[cursorY].Length;
                            lines[cursorY] += currentLine;
                            lines.RemoveAt(cursorY + 1);
                        }
                        break;
                    case ConsoleKey.Enter:
                        string newLine = lines[cursorY].Substring(cursorX);
                        lines[cursorY] = lines[cursorY].Substring(0, cursorX);
                        lines.Insert(cursorY + 1, newLine);
                        cursorY++;
                        cursorX = 0;
                        break;
                    default:
                        if (!char.IsControl(keyInfo.KeyChar))
                        {
                            lines[cursorY] = lines[cursorY].Insert(cursorX, keyInfo.KeyChar.ToString());
                            cursorX++;
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
                        foreach (string line in lines)
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
        }
    }
}