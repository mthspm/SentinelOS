using System;
using Cosmos.System.FileSystem.VFS;
using System.Collections.Generic;
using Cosmos.System.FileSystem;
using System.Security.Principal;


namespace SentinelOS.Resources
{
    public static class DiskExtensions
    {
        public static List<string> GetDiskInformation(this Disk disk)
        {
            List<string> diskInfo = new List<string>();
            diskInfo.Add($"Disk Size: {disk.Size / 1024 / 1024} MB");

            if (disk.Partitions.Count > 0)
            {
                for (int i = 0; i < disk.Partitions.Count; i++)
                {
                    var partition = disk.Partitions[i];
                    diskInfo.Add($"  Partition {i + 1}:");
                    diskInfo.Add($"    Block Size: {partition.Host.BlockSize} bytes");
                    diskInfo.Add($"    Block Count: {partition.Host.BlockCount}");
                    diskInfo.Add($"    Size: {partition.Host.BlockCount * partition.Host.BlockSize / 1024 / 1024} MB");
                }
            }
            else
            {
                diskInfo.Add("  No partitions found!");
            }
            return diskInfo;
        }
    }


    public static class DiskManager
    {
        private static List<Disk> disks;

        public static void Initialize()
        {
            try 
            { 
                disks = VFSManager.GetDisks();
                Console.WriteLine("DiskManager Initialized with" + disks.Count + " disks found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing DiskManager: " + ex.Message);
            }
        }

        public static Disk GetDisk(int index)
        {
            try
            {
                if (index < 0 || index >= disks.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
                return disks[index];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting disk: " + ex.Message);
                return null;
            }
        }

        public static void MountAllDisks()
        {
            try
            {
                foreach (var disk in disks)
                {
                    disk.Mount();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mounting disks: " + ex.Message);
            }
        }

        public static List<string> GetAllDisksInformation()
        {
            List<string> disksInfo = new List<string>();
            try
            {
                for (int i = 0; i < disks.Count; i++) 
                {
                    var diskInfo = GetDisk(i).GetDiskInformation();
                    diskInfo.Insert(0, $"Disk {i}");
                    disksInfo.AddRange(diskInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error displaying disks information: " + ex.Message);
            }
            return disksInfo;
        }

        public static void CreatePartitionOnDisk(int diskIndex, int size)
        {
            try
            {
                var disk = GetDisk(diskIndex);
                disk?.CreatePartition(size);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating partition: " + ex.Message);
            }
        }

        public static void DeletePartitionOnDisk(int diskIndex, int partitionIndex)
        {
            try
            {
                var disk = GetDisk(diskIndex);
                disk?.DeletePartition(partitionIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting partition: " + ex.Message);
            }
        }

        public static void ClearDisk(int diskIndex)
        {
            try
            {
                var disk = GetDisk(diskIndex);
                disk?.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing disk: " + ex.Message);
            }
        }

        public static void FormatPartitionOnDisk(int diskIndex, int partitionIndex, string format, bool quick = true)
        {
            try
            {
                var disk = GetDisk(diskIndex);
                disk?.FormatPartition(partitionIndex, format, quick);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error formatting partition: " + ex.Message);
            }
        }

        public static void MountPartitionOnDisk(int diskIndex, int partitionIndex)
        {
            try
            {
                var disk = GetDisk(diskIndex);
                disk?.MountPartition(partitionIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mounting partition: " + ex.Message);
            }
        }
    }
}