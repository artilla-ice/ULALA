﻿
using System;
using System.Collections.Generic;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Core.Contracts.Events
{
    public class NewMoneyInsertEventArgs
    {
        public MoneyInsertedEvent Response { get; set; }
    }

    public class NewMoneyInsertEvent : PubSubEvent<NewMoneyInsertEventArgs>
    {
    }
}
