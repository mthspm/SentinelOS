using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using Cosmos;

namespace SentinelOS.Resources
{
    /// <summary>
    /// Class that contains methods to interact with the system calls of the OS
    /// </summary>
    public static class SystemCalls
    {
        public static void Shutdown()
        {
            Console.WriteLine("Shutting down...");
            Sys.Power.Shutdown();
        }

        public static void Utils()
        {
            //Cosmos.Core.CPU ==> CPU INFORMATION
            //Cosmos.Core.CPU.GetAmountOfRAM();
            //Cosmos.Core.CPU.GetCPUBrandString();
            //Cosmos.Core.CPU.GetCPUCycleSpeed();
            //Cosmos.Core.CPU.GetCPUUptime();
            //Cosmos.Core.CPU.GetCPUVendorName();
            //Cosmos.Core.CPU.GetMemoryMap();
            //Cosmos.Core.CPU.GetEBPValue();
        }

        public static void Restart()
        {
            Console.WriteLine("Restarting...");
            Sys.Power.Reboot();
        }
    }
}
