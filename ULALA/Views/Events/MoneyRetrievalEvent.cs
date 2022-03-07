
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;

namespace ULALA.Infrastructure.Events
{
    public class MoneyRetrievalEventEventArgs
    {
        public MoneyRetrievalResponse Result { get; set; }
    }

    public class MoneyRetrievalEvent : PubSubEvent<MoneyRetrievalEventEventArgs>
    {
    }
}
