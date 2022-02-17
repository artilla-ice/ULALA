using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus.DTO.CashTotals;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;

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
                m_client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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

        public Status GetGeneralStatus()
        {
            OnCommand("requestStatus", "result"
                                , out Status result, 3);

            return result;
        }

        public CashTotalsResponse RequestCashTotals()
        {
            OnCommand("requestCashTotals", "result"
                                , out CashTotalsResponse result);

            return result;
        }

        public Task<MoneyRetrievalResponse> RetrieveStackerValues()
        {
            OnCommand("startRetrieveStackerCash", "event"
                                , out MoneyRetrievalResponse result);

            return Task.FromResult(result);
        }

        private void OnCommand<T>(string commandName, string jsonResponseValue, out T result, int id = 0, string version = "2.0")
        {
            result = default(T);

            if (m_client != null && m_client.Connected)
            {
                using (var networkStream = new NetworkStream(m_client))
                using (var streamWriter = new StreamWriter(networkStream, Encoding.ASCII))
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("version");
                        writer.WriteValue(version);
                        writer.WritePropertyName("method");
                        writer.WriteValue(commandName);
                        writer.WritePropertyName("id");
                        writer.WriteValue(id);
                    }
                    writer.WriteEndObject();
                }

                JsonSerializer serializer = new JsonSerializer();
                using (var networkStream = new NetworkStream(m_client))
                using (var streamWriter = new StreamReader(networkStream, new UTF8Encoding()))
                using (var reader = new JsonTextReader(streamWriter))
                {
                    var json = serializer.Deserialize(reader).ToString();
                    var jObject = JObject.Parse(json);
                    if (jObject != null)
                    {
                        var jToken = jObject.GetValue(jsonResponseValue);

                        if (jToken != null)
                            result = jToken.ToObject<T>();
                    }
                }
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

        private Socket m_client;
    }
}
