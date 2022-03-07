
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class ReceivedResponseEventArgs
    {
        public object Response { get; set; }

    }

    public class ReceivedResponseEvent : PubSubEvent<ReceivedResponseEventArgs>
    {
    }
}
