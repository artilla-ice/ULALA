using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus.DTO.CashTotals;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Infrastructure.PubSub;

namespace ULALA.Services.Zeus
{
    public class ZeusConnectionService : IZeusConnectionService
    {
        public ZeusConnectionService()
        {
        }


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

        public bool RequestMoneyInsertion()
        {
            bool result = false;

            if (m_client != null && m_client.Connected)
            {
                using (var networkStream = new NetworkStream(m_client))
                using (var streamWriter = new StreamWriter(networkStream, Encoding.ASCII))
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("version");
                        writer.WriteValue("2.0");
                        writer.WritePropertyName("method");
                        writer.WriteValue("startMoneyInsertion");
                        writer.WritePropertyName("params");
                        writer.WriteStartObject();
                        writer.WritePropertyName("amount");
                        writer.WriteValue(DefaultMaxInsertionAmount);
                        writer.WriteEndObject();
                        writer.WritePropertyName("id");
                        writer.WriteValue(1);
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
                        var jToken = jObject.GetValue("result");

                        if (jToken != null)
                        {
                            var response = jToken.ToString();
                            result = response == "ACK";
                        }
                    }
                }
            }

            return result;
        }

        public Task FinishMoneyInsertion()
        {
            OnCommand("finishInsertion", 2);

            //TODO: verificar el response del emulador que sea igual al dinero que se registro (en el viewmodel)

            return Task.CompletedTask;
        }

        public bool RequestDispenseSession(double amount)
        {
            bool result = false;

            if (m_client != null && m_client.Connected)
            {
                using (var networkStream = new NetworkStream(m_client))
                using (var streamWriter = new StreamWriter(networkStream, Encoding.ASCII))
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("version");
                        writer.WriteValue("2.0");
                        writer.WritePropertyName("method");
                        writer.WriteValue("startDispenseSession");
                        writer.WritePropertyName("params");
                        writer.WriteStartObject();
                        writer.WritePropertyName("amount");
                        writer.WriteValue(amount);
                        writer.WriteEndObject();
                        writer.WritePropertyName("id");
                        writer.WriteValue(1);
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
                        var jToken = jObject.GetValue("result");

                        if (jToken != null)
                        {
                            var response = jToken.ToString();
                            result = response == "ACK";
                        }
                    }
                }
            }

            return result;
        }

        public Task FinishDispenseSession()
        {
            OnCommand("finishDispenseSession", 2);

            return Task.CompletedTask;
        }

        public Task<MoneyMovementEvent> OnStartListeningForEvent()
        {
            MoneyMovementEvent result = null;
            try
            {
                if (m_client == null || !m_client.Connected)
                    return null;
                
                JsonSerializer serializer = new JsonSerializer();
                using (var networkStream = new NetworkStream(m_client))
                using (var streamWriter = new StreamReader(networkStream, new UTF8Encoding()))
                using (var reader = new JsonTextReader(streamWriter))
                {
                    var json = serializer.Deserialize(reader).ToString();
                    var jObject = JObject.Parse(json);
                    if (jObject != null)
                    {
                        var jToken = jObject.GetValue("event");

                        if (jToken != null)
                            result = jToken.ToObject<MoneyMovementEvent>();
                    }
                
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Task.FromResult(result);
        }

        public Status GetGeneralStatus()
        {
            OnCommandWithResponse("requestStatus", "result"
                                , out Status result, 3);

            return result;
        }

        public CashTotalsResponse RequestCashTotals()
        {
            OnCommandWithResponse("requestCashTotals", "result"
                                , out CashTotalsResponse result);

            return result;
        }

        public Task<MoneyRetrievalResponse> RetrieveStackerValues()
        {
            OnCommandWithResponse("startRetrieveStackerCash", "event"
                                , out MoneyRetrievalResponse result);

            return Task.FromResult(result);
        }

        private void OnCommand(string commandName, int id = 0, string version = "2.0")
        {
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
            }
        }

        private void OnCommandWithResponse<T>(string commandName, string jsonResponseValue, out T result, int id = 0, string version = "2.0")
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
                    try
                    {
                        var json = serializer.Deserialize<string>(reader);
                        var jObject = JObject.Parse(json);
                        if (jObject != null)
                        {
                            var jToken = jObject.GetValue(jsonResponseValue);

                            if (jToken != null)
                                result = jToken.ToObject<T>();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
        private static int DefaultMaxInsertionAmount = 1000000;
    }
}
