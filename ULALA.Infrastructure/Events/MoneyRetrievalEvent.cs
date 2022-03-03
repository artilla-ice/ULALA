
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class MoneyRetrievalEventEventArgs
    {
    }

    public class MoneyRetrievalEvent : PubSubEvent<MoneyRetrievalEventEventArgs>
    {
    }
}
