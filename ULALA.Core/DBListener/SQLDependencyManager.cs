
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
    public class SQLDependencyManager : ISQLDependencyManager
    {
        [Dependency]
        public IZeusConnectionService ZeusConnectionService { get; set; }

        [Dependency]
        public INavigationManager NavigationManager { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public SQLDependencyManager()
        {
        }

        public SQLDependencyManager(IEventAggregator eventAggregator) 
        {
            this.EventAggregator = eventAggregator;
        }

        public Task Initialize()
        {
            SetTimer();

            return Task.CompletedTask;
        }

        public void StartListening(Object state)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    String sql = this.GETINCOMINGCHARGESAMOUNTCOMMAND;

                    bool existRowsToDelete = false;
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existRowsToDelete = false;

                                var chargeId = reader.GetInt32(0);
                                if (chargeId != -1)
                                {
                                    existRowsToDelete = true;
                                    m_timer.Change(Timeout.Infinite, Timeout.Infinite);

                                    var chargeAmount = reader.GetDouble(1);
                                    OnStartNewChargeView(chargeAmount);
                                }
                            }
                        }
                    }

                    if(existRowsToDelete)
                    {
                        using (SqlCommand deleteCmd = new SqlCommand(DELETEALLINCOMINGCHARGES, connection))
                        {
                            deleteCmd.ExecuteNonQuery();
                            m_timer.Change(0, 1000);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                throw e;
            }

        }
        
        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(this.CONNECTION_STRING);
            connection.Open();

            return connection;
        }

        private async void OnStartNewChargeView(double args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
               {
                    var param = new Dictionary<string, object>() { { "TotalChargeAmount", args } };
                    this.NavigationManager.NavigateTo(ViewNames.NewCharge, param);
               });
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            m_timer = new Timer(StartListening, null, 0, 1000);
        }

        private static Timer m_timer;

        private readonly string CONNECTION_STRING = "Data Source=LAPTOP-S5FBM5U2;Initial Catalog=ULALA;Persist Security Info=True;User ID=sa;Password=gttbhr";
        private readonly string GETINCOMINGCHARGESAMOUNTCOMMAND = "SELECT TOP 1 * FROM [ULALA].[dbo].[ChargeIncoming_temp] ORDER BY [Id] DESC";
        private readonly string DELETEALLINCOMINGCHARGES = "DELETE FROM [ULALA].[dbo].[ChargeIncoming_temp]";
    }
}
