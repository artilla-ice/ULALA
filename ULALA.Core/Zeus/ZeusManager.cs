
using ULALA.Core.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus;
using Unity;

namespace ULALA.Core.ZEUS
{
    public class ZeusManager : IZeusManager
    {
        [Dependency]
        public IZeusConnectionService ZeusConnectionService { get; set; }

        public ZeusManager() 
        { }

        public void OnStartListening()
        {
            this.ZeusConnectionService.StartListening();
        }

        public void OnCloseConnection()
        {
            this.ZeusConnectionService.StopComm();
        }

        public void GetCashTotals()
        {
            this.ZeusConnectionService.RequestCashTotals();
        }

        public bool IsConnected => this.ZeusConnectionService.IsConnected;
    }


}
