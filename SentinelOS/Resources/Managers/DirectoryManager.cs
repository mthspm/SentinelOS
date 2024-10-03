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
using SentinelOS.Resources.Handlers;
using SentinelOS.GUI.Windows;

namespace SentinelOS.Resources.Managers
{
    /// <summary>
    /// SentinelOS directory manager class that contains methods to manage directories.
    /// </summary>
    class DirectoryManager
    {
        public static string CurrentPath { get; set; } = Paths.Root;

        private readonly static List<string> defaultDirectories = new List<string>
                {
                    Paths.Root,
                    Paths.System,
                    Paths.ProgramFiles,
                    Paths.Temp,
                    Paths.User,
                    Paths.Desktop,
                };

        /// <summary>
        /// Creates the system files and directories.
        /// </summary>
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

        /// <summary>
        /// Creates a directory with the specified name.
        /// </summary>
        /// <param name="name">The name of the directory to create.</param>
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
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        /// <summary>
        /// Deletes the directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
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
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
            }
        }

        /// <summary>
        /// Retrieves the list of directory entries in the current directory.
        /// </summary>
        /// <returns>A list of DirectoryEntry objects representing the directory entries.</returns>
        public static List<DirectoryEntry> GetDirectoryEntries()
        {
            try
            {
                return VFSManager.GetDirectoryListing(CurrentPath);
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
                return new List<DirectoryEntry>();
            }
        }

        /// <summary>
        /// Retrieves the list of directory entries in the specified directory.
        /// </summary>
        /// <param name="apath">The path of the directory.</param>
        /// <returns>A list of DirectoryEntry objects representing the directory entries.</returns>
        public static List<DirectoryEntry> GetDirectoryEntries(string apath)
        {
            try
            {
                return VFSManager.GetDirectoryListing(apath);
            }
            catch (Exception e)
            {
                AlertHandler.DisplayAlert(AlertType.Error, e.Message);
                return new List<DirectoryEntry>();
            }
        }

        /// <summary>
        /// Lists the contents of the current directory.
        /// </summary>
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

        /// <summary>
        /// Changes the current directory to the specified directory.
        /// </summary>
        /// <param name="name">The name of the directory to change to.</param>
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

        public static void MoveToParentDirectory()
        {
            if (CurrentPath == Paths.Root)
            {
                AlertHandler.DisplayAlert(AlertType.Warning, "You are already in the root directory");
                return;
            }
            CurrentPath = CurrentPath.Remove(CurrentPath.LastIndexOf(@"\"));
        }

        /// <summary>
        /// Clears the current directory.
        /// </summary>
        /// <param name="recursive">`true` to clear all subdirectories and files; `false` to clear only files.</param>
        public static void ClearDir(bool recursive)
        {
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
                    string msg = $"Error deleting {entry.mFullPath}: {e.Message}";
                    AlertHandler.DisplayAlert(AlertType.Error, msg);
                }
            }
            Console.WriteLine("Directory cleared");
        }

        /// <summary>
        /// Checks if the specified path is a valid system path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path is a valid system path; otherwise, false.</returns>
        public static bool IsValidSystemPath(string path)
        {
            return path.StartsWith(Paths.Root, StringComparison.OrdinalIgnoreCase);
        }
    }

}
