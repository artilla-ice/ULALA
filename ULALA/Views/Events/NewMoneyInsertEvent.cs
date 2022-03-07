
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class NewMoneyInsertEventArgs
    {
        public MoneyMovementEvent Response { get; set; }
    }

    public class NewMoneyInsertEvent : PubSubEvent<NewMoneyInsertEventArgs>
    {
    }
}
