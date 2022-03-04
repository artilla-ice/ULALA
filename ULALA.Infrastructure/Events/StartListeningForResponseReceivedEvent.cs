
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class StartListeningForResponseReceivedEventArgs
    {
        public string EvenType { get; set; }
        public string Response { get; set; }
    }

    public class StartListeningForResponseReceivedEvent : PubSubEvent<StartListeningForResponseReceivedEventArgs>
    {
    }
}
