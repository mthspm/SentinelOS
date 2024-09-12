using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using SentinelOS.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources
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
                Console.WriteLine(e.ToString());
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
                Console.WriteLine(e.Message);
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
                Console.WriteLine(e.Message);
            }
        }

    }
}
