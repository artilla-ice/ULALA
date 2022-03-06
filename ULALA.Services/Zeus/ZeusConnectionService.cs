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
using System.Collections.Generic;
using System.Linq;
using ULALA.Services.Contracts.Zeus.DTO.CashDispension;

namespace ULALA.Services.Zeus
{
    public class ZeusConnectionService : IZeusConnectionService
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public ZeusConnectionService()
        {
        }

        public ZeusConnectionService(IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
        }

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public bool IsConnected { get => m_client != null && m_client.Connected; }

        public Task Initialize()
        {
            SubscribeToEvents();

            this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs());
            return Task.CompletedTask;
        }

        public bool StartListening()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());//IPAddress.Parse("10.37.140.220");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2021);//new IPEndPoint(ipAddress, 1989);

            try
            {
                m_client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_client.Connect(localEndPoint);

                if (m_client.Connected)
                    this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs());


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

        public Task RequestMoneyInsertion()
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

                this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs
                {
                    Response = "result",
                    EvenType = "commandResponse",
                    ResponseId = 1
                });

            }

            return Task.CompletedTask;
        }

        public Task FinishMoneyInsertion()
        {
            OnCommand("finishInsertion", 2);

            //TODO: verificar el response del emulador que sea igual al dinero que se registro (en el viewmodel)

            return Task.CompletedTask;
        }

        public Task<bool> RequestDispenseSession(double amount)
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
                        writer.WriteValue(12);
                    }
                    writer.WriteEndObject();
                }

                this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs
                {
                    Response = "result",
                    EvenType = "commandResponse",
                    ResponseId = 12
                });
            }

            return Task.FromResult(result);
        }

        public Task FinishDispenseSession()
        {
            OnCommand("finishDispenseSession", 2);

            this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs
            {
                Response = "result",
                EvenType = "commandResponse",
                ResponseId = 12
            });

            return Task.CompletedTask;
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

                result = default(T); //await WaitingResponse(jsonResponseValue, "");
            }

            return result;
        }

        private Task<ResponseReceivedEventArgs> WaitingResponse(string jsonResponseValue, string eventType, int id = -1)
        {
            ResponseReceivedEventArgs result = null;

            int responseId = id;

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

                        var json = serializer.Deserialize(reader).ToString();//\"totalMoneyInserted
                        var jObject = JObject.Parse(json);
                        if (jObject != null)
                        {
                            var jToken = jObject.GetValue(jsonResponseValue);
                            if (jToken == null)
                            {
                                jsonResponseValue = "result";
                                eventType = "commandResponse";

                                jToken = jObject.GetValue(jsonResponseValue);

                                if (jToken == null)
                                {
                                    jsonResponseValue = "event";
                                    jToken = jObject.GetValue(jsonResponseValue);
                                    eventType = jToken.Value<string>("type");
                                }
                            }

                            if (jToken != null)
                            {
                                //verify eventType
                                if(jsonResponseValue == "event"
                                     && eventType != jToken.Value<string>("type"))
                                {
                                    eventType = jToken.Value<string>("type");
                                }

                                object objResult = null;
                                if (eventType == "moneyMovementEvent")
                                {
                                    objResult = jToken.ToObject<MoneyMovementEvent>();
                                }
                                else if (eventType == "moneyRetrieval")
                                {
                                    var args = jToken.ToObject<MoneyRetrievalResponse>();
                                    this.EventAggregator.GetEvent<MoneyRetrievalEvent>()
                                        .Publish(new MoneyRetrievalEventEventArgs() { Result =  args});

                                }
                                else if (eventType == "commandResponse")
                                {
                                    var valueId = jObject.GetValue("id");
                                    responseId = valueId.ToObject<int>();

                                    var response = jToken.ToString();
                                    if (response != null)
                                    {
                                        objResult = (bool)(response == "ACK");
                                        if (!(bool)objResult && response != "NACK")
                                        {
                                            var resultJObject = JObject.Parse(response);
                                            if (resultJObject != null)
                                            {
                                                var objProperties = resultJObject.Properties().ToList();
                                                var firstPropertyName = objProperties.ElementAt(0).Name;
                                                if (firstPropertyName == "totalMoneyInserted")
                                                {
                                                    objResult = jToken.ToObject<FinishInsertionResponse>();
                                                }
                                                else if (firstPropertyName == "totalMoneyDispensed")
                                                {
                                                    objResult = jToken.ToObject<FinishDispenseResponse>();
                                                    eventType = firstPropertyName;
                                                }
                                            }
                                        }

                                    }
                                }
                                else if (eventType == "moneyInsertedEvent" || eventType == "moneyDispensedEvent")
                                {
                                    objResult = jToken.ToObject<MoneyMovementEvent>();
                                }

                                result = new ResponseReceivedEventArgs()
                                {
                                    ResponseId = responseId,
                                    CommandId = eventType,
                                    Result = objResult
                                };
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return Task.FromResult(result);
        }

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>()
                .Subscribe(async (args) =>
               {
                   while (IsConnected)
                   {
                       var result = await WaitingResponse(args.Response, args.EvenType);
                       if (result != null)
                       {
                           this.EventAggregator.GetEvent<ResponseReceivedEvent>().Publish(new ResponseReceivedEventArgs
                           {
                               ResponseId = result.ResponseId,
                               CommandId = result.CommandId,
                               Result = result.Result
                           });
                       }
                   }

               }, ThreadOption.BackgroundThread);
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
        private static IDictionary<string, Type> m_responseTypesMap = new Dictionary<string, Type>()
        {
            { "moneyMovementEvent", typeof(MoneyMovementEvent) },
            { "boolean", typeof(bool) }

        };
    }
}
