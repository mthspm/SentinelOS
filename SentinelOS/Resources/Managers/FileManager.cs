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

        public static void WriteLinesToFile(string fullPath, List<string> lines)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
                Console.WriteLine("\nFile saved with success at " + fullPath);
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        public static void CreateEmptyFile(string name)
        {
            name = CleanFileName(name);

            if (string.IsNullOrEmpty(name))
            {
                AlertHandler.DisplayAlert(AlertType.Error, "Invalid file name.");
                return;
            }

            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                VFSManager.CreateFile(path);
                Console.WriteLine("File created with success");
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        private static string CleanFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            char[] invalidChars = VFSManager.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                name = name.Replace(c.ToString(), string.Empty);
            }

            name = name.Trim();
            name = name.Trim('.');
            return name;
        }

        public static string ReadFile(string name)
        {
            string path = DirectoryManager.CurrentPath + @"\" + name;
            try
            {
                var fileContent = File.ReadAllText(path);
                return fileContent;
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
                if (VFSManager.FileExists(oldPath))
                {
                    var fileStream = VFSManager.GetFile(oldPath).GetFileStream();
                    var newFileStream = VFSManager.CreateFile(newPath).GetFileStream();

                    fileStream.CopyTo(newFileStream);
                    fileStream.Close();
                    newFileStream.Close();

                    VFSManager.DeleteFile(oldPath);
                    AlertHandler.DisplayAlert(AlertType.Success, "File renamed with success!");
                    return true;
                }

                AlertHandler.DisplayAlert(AlertType.Error, "File not found!");
                return false;

            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
                return false;
            }
        }

    }
}
