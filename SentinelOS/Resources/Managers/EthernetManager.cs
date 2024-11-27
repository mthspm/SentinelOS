using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cosmos.HAL;
using Cosmos.System.Network;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;

namespace SentinelOS.Resources.Managers
{
    static class EthernetManager
    {
        private static NetworkDevice nic;

        static EthernetManager()
        {
            nic = NetworkDevice.GetDeviceByName("eth0");
            if (nic != null)
            {
                IPConfig.Enable(nic, new Address(192, 168, 1, 69), new Address(255, 255, 255, 0),
                    new Address(192, 168, 1, 254));
            }
        }

        public static void ConfigureDHCP()
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }

            using (var dhcpClient = new DHCPClient())
            {
                dhcpClient.SendDiscoverPacket();
            }
        }

        public static string GetLocalIPAddress()
        {
            return NetworkConfiguration.CurrentAddress.ToString();
        }

        public static void ResetIPConfig()
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }

            IPConfig.Enable(nic, new Address(0, 0, 0, 0), new Address(0, 0, 0, 0), new Address(0, 0, 0, 0));
        }

        public static bool IsConnected()
        {
            return nic != null && nic.Ready;
        }

        public static List<string> GetInfo()
        {
            var info = new List<string>();
            if (nic == null)
            {
                info.Add("Network device not found.");
                return info;
            }
            info.Add($"Network Info");
            info.Add($"Device Name: {nic.Name}");
            info.Add($"Device NameID: {nic.NameID}");
            info.Add($"MAC Address: {nic.MACAddress.ToString()}");
            info.Add($"IP Address: {NetworkConfiguration.CurrentAddress.ToString()}");
            info.Add($"Subnet Mask: {NetworkConfiguration.CurrentNetworkConfig.IPConfig.SubnetMask.ToString()}");
            info.Add($"Default Gateway: {NetworkConfiguration.CurrentNetworkConfig.IPConfig.DefaultGateway.ToString()}");
            info.Add($"Ready: {nic.Ready.ToString()}");
            return info;
            
        }

        // UDP and TCP methods

        public static void SendUDP(string host, int port, string message)
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }
            using (var client = new UdpClient())
            {
                client.Connect(host, port);
                var data = Encoding.ASCII.GetBytes(message);
                client.Send(data, data.Length);
            }
        }

        public static string ReceiveUDP(int listenPort)
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }
            using (var client = new UdpClient(listenPort))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
                var receivedData = client.Receive(ref remoteEndPoint);
                return Encoding.ASCII.GetString(receivedData);
            }
        }

        public static void SendTCP(string host, int port, string message)
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }
            using (var client = new TcpClient())
            {
                client.Connect(host, port);
                var data = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
            }
        }

        public static string ReceiveTCP(int listenPort)
        {
            if (nic == null)
            {
                throw new Exception("Network device not found.");
            }
            var localAddr = IPAddress.Any;
            var listener = new TcpListener(localAddr, listenPort);
            listener.Start();
            using (var client = listener.AcceptTcpClient())
            {
                var receivedData = new byte[client.Available];
                client.GetStream().Read(receivedData, 0, receivedData.Length);
                return Encoding.ASCII.GetString(receivedData);
            }
        }
    }
}
