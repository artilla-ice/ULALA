using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus;

namespace ULALA.Services.Zeus
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }

    public class ZeusConnectionService : IZeusConnectionService
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public bool IsConnected { get => m_client != null && m_client.Connected; }

        public bool StartListening()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2021);

            try
            {
                m_client = new TcpClient();
                m_client.Connect(localEndPoint);

                return m_client.Connected;
            }
            catch
            {
                return false;
            }
        }

        public void StopComm()
        {
            if(m_client != null && m_client.Connected)
            {
                m_client.Close();
            }
        }

        public void RequestCashTotals()
        {
            using (var streamWriter = new StreamWriter(m_client.GetStream(), Encoding.ASCII))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                {
                    writer.WritePropertyName("version");
                    writer.WriteValue("2.0");
                    writer.WritePropertyName("method");
                    writer.WriteValue("requestCashTotals");
                    writer.WritePropertyName("id");
                    writer.WriteValue(0);
                }
                writer.WriteEndObject();
            }
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private TcpClient m_client;
    }
}
