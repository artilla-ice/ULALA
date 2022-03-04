
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class ResponseReceivedEventArgs
    {
        public string CommandId { get; set; }
        public int ResponseId { get; set; }
        public object Result { get; set; }

    }

    public class ResponseReceivedEvent : PubSubEvent<ResponseReceivedEventArgs>  
    {
    }
}
