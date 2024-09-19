using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.FileSystem.Listing;
using System.IO;
using System.Xml.Linq;
using System.IO.Enumeration;

namespace SentinelOS.Resources
{
    class DirectoryManager
    {
        public static string CurrentPath { get; set; } = Paths.Root;

        private static List<string> defaultDirectories = new List<string>
            {
                Paths.Root,
                Paths.System,
                Paths.ProgramFiles,
                Paths.Temp,
                Paths.User,
                Paths.Desktop,
            };

        public static void CreateSystemFiles()
        {
            CosmosVFS vfs = new CosmosVFS();
            VFSManager.RegisterVFS(vfs);
            foreach (string dir in defaultDirectories)
            {
                if (!VFSManager.DirectoryExists(dir))
                {
                    VFSManager.CreateDirectory(dir);
                }
            }
            CurrentPath = Paths.Desktop;
        }

        public static void CreateDir(string name)
        {
            string dirPath = CurrentPath + @"\" + name;
            try
            {
                if (VFSManager.DirectoryExists(dirPath))
                {
                    Console.WriteLine("Directory already exists");
                }
                else
                {
                    VFSManager.CreateDirectory(dirPath);
                    Console.WriteLine("Directory created with success");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteDir(string path)
        {
            try
            {
                if (VFSManager.DirectoryExists(CurrentPath + @"\" + path))
                {
                    path = CurrentPath + @"\" + path;
                }
                VFSManager.DeleteDirectory(path, true);
                Console.WriteLine("Directory deleted with success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ListDir()
        {
            var directoryListing = VFSManager.GetDirectoryListing(CurrentPath);
            if (directoryListing.Count == 0)
            {
                Console.WriteLine("Directory is empty");
            }
            else
            {
                Console.WriteLine("Contents of the directory:");
                foreach (DirectoryEntry item in directoryListing)
                {
                    Console.WriteLine(item.mFullPath);
                }
            }
        }

        public static void ChangeDir(string name)
        {
            if (VFSManager.DirectoryExists(CurrentPath + @"\" + name))
            {
                CurrentPath = CurrentPath + @"\" + name;
            }
            else if (VFSManager.DirectoryExists(name))
            {
                CurrentPath = name;
            }
            else if (name == "..")
            {
                if (CurrentPath != Paths.Root)
                {
                    CurrentPath = CurrentPath.Remove(CurrentPath.LastIndexOf(@"\"));
                }
            }
            else
            {
                Console.WriteLine("Directory not found");
            }
        }
        /// <summary>
        /// Clear the current directory
        /// </summary>
        /// <param name="recursive"> `true` to clear all subdirectories and files ; `false` to clear only files</param>
        public static void ClearDir(bool recursive)
        {
            // Get all files and directories in the current directory
            var directoryListing = VFSManager.GetDirectoryListing(CurrentPath);
            foreach (var entry in directoryListing)
            {
                try
                {
                    if (entry.mEntryType == DirectoryEntryTypeEnum.Directory)
                    {
                        Directory.Delete(entry.mFullPath, recursive);
                    }
                    else
                    {
                        File.Delete(entry.mFullPath);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error deleting {entry.mFullPath}: {e.Message}");
                }
            }
            Console.WriteLine("Directory cleared");
        }

        public static bool IsValidSystemPath(string path)
        {
            return path.StartsWith(Paths.Root, StringComparison.OrdinalIgnoreCase);
        }

    }

}
