using Cosmos;
using SentinelOS.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources
{
    class ConsoleManager
    {
        //This class will handle all console related operations, like ls, cd, mkdir, etc.

        private static Dictionary<string, Action<string>> commandMap = new Dictionary<string, Action<string>>()
        {
            { "ls", (string arg) => ListDirectory() },
            { "cd", (string arg) => ChangeDirectory(arg) },
            { "mkdir", (string arg) => MakeDirectory(arg) },
            { "cldir", (string arg) => DirectoryManager.ClearDir() },
            { "rmdir", (string arg) => RemoveDirectory(arg) },
            { "nano", (string arg) => Nano.Main(arg) },
            { "touch", (string arg) => CreateFile(arg) },
            { "rm", (string arg) => RemoveFile(arg) },
            { "clear", (string arg) => ClearConsole() },
            { "cat", (string arg) => PrintFileContent(arg) },
            { "echo", (string arg) => Echo(arg) },
            { "help", (string arg) => ConsoleHelp(arg) },
            { "pwd", (string arg) => PrintWorkingDirectory() }
        };

        private static Dictionary<string, string> commandHelpMap = new Dictionary<string, string>()
            {
                { "ls", "List files and directories in the current directory." },
                { "cd", "args: <path> - Change the current directory." },
                { "mkdir", "args: <name> - Create a new directory." },
                { "rmdir", "args: <name> - Remove a directory." },
                { "rm", "args: <name> - Remove a file." },
                { "touch", "args: <name> - Create a new file." },
                { "clear", "Clear the console." },
                { "cat", "args: <name> - Print the content of a file." },
                { "echo", "args: <content> - Print the content to the console." },
                { "help", "Print this help text." },
                { "pwd", "Print the current working directory." }
            };

        private static void ConsoleHelp(string arg)
        {
            if (!string.IsNullOrEmpty(arg) && commandMap.ContainsKey(arg))
            {
                // Print the help text for the specific command if found
                if (commandHelpMap.TryGetValue(arg, out string helpText))
                {
                    Console.WriteLine(helpText);
                }
                else
                {
                    Console.WriteLine("Help text not available for the command: " + arg);
                }
            }
            else if (string.IsNullOrEmpty(arg))
            {
                // If no argument is provided, print the available commands
                Console.WriteLine("Available commands:");
                foreach (KeyValuePair<string, string> entry in commandHelpMap)
                {
                    Console.WriteLine(entry.Key + " - " + entry.Value);
                }
            }
            else
            {
                Console.WriteLine("Command not found.");
            }
        }

        private static void ListDirectory()
        {
            DirectoryManager.ListDir();
        }

        private static void ChangeDirectory(string name)
        {
            DirectoryManager.ChangeDir(name);
        }

        private static void MakeDirectory(string name)
        {
            DirectoryManager.CreateDir(name);
        }

        private static void RemoveDirectory(string name)
        {
            DirectoryManager.DeleteDir(name);
        }

        private static void CreateFile(string name)
        {
            FileManager.CreateFile(name, "empty file");
        }

        private static void RemoveFile(string name)
        {
            FileManager.DeleteFile(name);
        }

        private static void PrintFileContent(string name)
        {
            string content = FileManager.ReadFile(name);
            if (content != null)
            {
                Console.WriteLine(content);
            }
        }

        private static void ClearConsole()
        {
            Console.Clear();
        }

        private static void Echo(string content)
        {
            Console.WriteLine(content);
        }

        private static void PrintWorkingDirectory()
        {
            Console.WriteLine(DirectoryManager.CurrentPath);
        }

        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            string[] commandParts = command.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandParts.Length == 0)
            {
                return;
            }
            string commandName = commandParts[0];
            string commandArg = commandParts.Length > 1 ? commandParts[1] : null;

            if (commandMap.ContainsKey(commandName))
            {
                commandMap[commandName](commandArg);
            }
            else
            {
                Console.WriteLine("Command not found.");
            }
        }


    }
}
