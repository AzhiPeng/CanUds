using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.测试类
{
    public class PingWithSpecificAdapter
    {
        // 定义ICMP回显请求和回显回复的数据结构
         public  static int PingFromSpecificIp(string sourceIp, string destinationIp)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(sourceIp), 0));

                byte[] icmpPacket = CreateIcmpEchoRequest();

                EndPoint endPoint = new IPEndPoint(IPAddress.Parse(destinationIp), 0);

                try
                {
                    socket.SendTo(icmpPacket, endPoint);

                    byte[] buffer = new byte[64];
                    socket.ReceiveTimeout = 4000; // Set timeout to 3 seconds

                    int receivedBytes = socket.ReceiveFrom(buffer, ref endPoint);
                    if (receivedBytes > 0)
                    {
               //         Console.WriteLine($"Response from {destinationIp} to {sourceIp} received.");
                        return receivedBytes;
                    }
                }
                catch (Exception ex)
                {
                 //   Console.WriteLine($"Error pinging from {sourceIp}: {ex.Message}");
                    return 0;
                }
                return 0;
            }
        }

        static byte[] CreateIcmpEchoRequest()
        {
            byte[] packet = new byte[8 + 32]; // ICMP Header (8 bytes) + Data (32 bytes)

            // ICMP Header
            packet[0] = 8; // Type: Echo Request
            packet[1] = 0; // Code
            packet[2] = 0; // Checksum (initially zero)
            packet[3] = 0; // Checksum (initially zero)
            packet[4] = 0; // Identifier
            packet[5] = 0; // Identifier
            packet[6] = 0; // Sequence Number
            packet[7] = 0; // Sequence Number
           // Fill data part with some random bytes
           // new Random().NextBytes(packet.AsSpan(8));
            ushort checksum = ComputeChecksum(packet);
            packet[2] = (byte)(checksum >> 8);
            packet[3] = (byte)(checksum & 0xFF);

            return packet;
        }

        static ushort ComputeChecksum(byte[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                ushort word = (ushort)((data[i] << 8) + (data[i + 1]));
                sum += word;
            }

            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }

            return (ushort)~sum;
        }
    }
}
