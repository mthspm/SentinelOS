using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using SentinelOS.GUI.Windows;
using SentinelOS.Resources.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources.Managers
{
    class FileManager
    {

        public static void CreateFile(string name, string content)
        {
            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                File.Create(path);
                File.WriteAllText(path, content);
                Console.WriteLine("File created with success");
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        public static void WriteLinesToFile(string fullpath, List<string> lines)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fullpath))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
                Console.WriteLine("\nFile saved with success at " + fullpath);
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        public static void CreateEmptyFile(string name)
        {
            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                File.Create(path);
                Console.WriteLine("File created with success");
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        public static string ReadFile(string name)
        {
            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                var file_content = File.ReadAllText(path);
                return file_content;
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
                return null;
            }
        }

        public static void DeleteFile(string name)
        {
            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                File.Delete(path);
                Console.WriteLine("File deleted with success");
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        public static bool RenameFile(string oldName, string newName)
        {
            string oldPath = DirectoryManager.CurrentPath + @"\" + oldName;
            string newPath = DirectoryManager.CurrentPath + @"\" + newName;
            try
            {
                File.Move(oldPath, newPath);
                Console.WriteLine("File renamed with success");
                return true;
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
                return false;
            }
        }

    }
}
