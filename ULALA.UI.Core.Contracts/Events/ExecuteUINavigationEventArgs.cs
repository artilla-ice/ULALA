using System;
using ULALA.Infrastructure.PubSub;
using ULALA.UI.Core.Contracts.Navigation;

namespace ULALA.UI.Core.Contracts.Events
{
    public class ExecuteUINavigationEventArgs
    {
        public MainViewType ViewType { get; set; }
    }


    public class ExecuteUINavigationEvent : PubSubEvent<ExecuteUINavigationEventArgs>
    {
    }
}
