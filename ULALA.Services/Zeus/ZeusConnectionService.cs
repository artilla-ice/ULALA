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
using ULALA.Infrastructure.Events;
using Unity;

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
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());//IPAddress.Parse("10.37.140.220");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2021);//new IPEndPoint(ipAddress, 1989);

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
            if (m_client != null && m_client.Connected)
            {
                m_client.Close();
            }
        }

        public async Task<bool> RequestMoneyInsertion()
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

                result = await WaitingResponse<bool>("result");

            }

            return result;
        }

        public Task FinishMoneyInsertion()
        {
            OnCommand("finishInsertion", 2);

            //TODO: verificar el response del emulador que sea igual al dinero que se registro (en el viewmodel)

            return Task.CompletedTask;
        }

        public async Task<bool> RequestDispenseSession(double amount)
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

                result = await Task.Run(async () =>  await WaitingResponse<bool>("result"));
            }

            return result;
        }

        public Task FinishDispenseSession()
        {
            OnCommand("finishDispenseSession", 2);  

            return Task.CompletedTask;
        }

        public async Task<T> OnStartListeningForEvent<T>(string expectedResponse = "event")
        {
            return await WaitingResponse<T>(expectedResponse);
        }

        public async Task<Status> GetGeneralStatus()
        {
            return await OnCommandWithResponse<Status>("requestStatus", "result", 3);
        }

        public async Task<CashTotalsResponse> RequestCashTotals()
        {
            return await OnCommandWithResponse<CashTotalsResponse>("requestCashTotals", "result");
        }

        public async Task<MoneyRetrievalResponse> RetrieveStackerValues()
        {
            return await OnCommandWithResponse<MoneyRetrievalResponse>("startRetrieveStackerCash", "event");
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

        private async Task<T> OnCommandWithResponse<T>(string commandName, string jsonResponseValue, int id = 0, string version = "2.0")
        {
            T result = default(T);
            if (m_client != null && m_client.Connected)
            {
                OnCommand(commandName, id, version);

                result = await WaitingResponse<T>(jsonResponseValue);
            }

            return result;
        }

        private  Task<T> WaitingResponse<T>(string jsonResponseValue)
        {
            T result = default(T);

            JsonSerializer serializer = new JsonSerializer();
            using (var networkStream = new NetworkStream(m_client))
            {
                using (var streamWriter = new StreamReader(networkStream, new UTF8Encoding()))
                using (var reader = new JsonTextReader(streamWriter))
                {
                    try
                    {
                        if (!networkStream.DataAvailable)
                            return Task.FromResult(result);

                        var json = serializer.Deserialize(reader).ToString();
                        var jObject = JObject.Parse(json);
                        if (jObject != null)
                        {
                            var jToken = jObject.GetValue(jsonResponseValue);
                            if (jToken != null)
                            {
                                if (jsonResponseValue == "result")
                                {
                                    var response = jToken.ToString();
                                    result = (T)(object)(response == "ACK");
                                }
                                else if (jsonResponseValue == "event")
                                    result = jToken.ToObject<T>();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }


            return Task.FromResult(result);
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
