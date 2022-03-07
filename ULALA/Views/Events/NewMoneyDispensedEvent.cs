
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Infrastructure.Events
{
    public class NewMoneyDispensedEventArgs
    {
        public MoneyMovementEvent Response { get; set; }
    }

    public class NewMoneyDispensedEvent : PubSubEvent<NewMoneyDispensedEventArgs>
    {
    }
}
