
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Events;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;
using ULALA.UI.Core.Contracts.Navigation;
using Unity;
using Windows.UI.Core;

namespace ULALA.Core.Zeus
{
    public class LoggerManager : ILoggerManager
    {
        [Dependency]
        public INavigationManager NavigationManager { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public LoggerManager()
        {
        }

        public LoggerManager(IEventAggregator eventAggregator) 
        {
            this.EventAggregator = eventAggregator;
        }

        public Task Initialize()
        {


            return Task.CompletedTask;
        }

        public void WriteInfo(string module, string action = "", string message = "")
        {

        }

        private readonly string INSERT_ACTION_LOG = "INSERT INTO [ActionsLogs] VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})"; 
    }
}
