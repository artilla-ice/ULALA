
using System.Collections.Generic;
using System.Linq;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Zeus;
using Unity;

namespace ULALA.Core.Zeus
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

        public IEnumerable<WithdrawalCashModel> GetRecyclerValues()
        {
            var cashTotals = this.ZeusConnectionService.RequestCashTotals();
            if (!ZeusConnectionService.IsConnected)
                return null;

            var recyclerValues = cashTotals.CashTotals.RecyclersInfo.RecyclerBills
                                    .Select(r => new WithdrawalCashModel()
                                    {
                                        CashType = CashType.Bills,
                                        Denomination = r.Value,
                                        RecyclerQuantity = r.Count
                                    }).ToList() ;

            recyclerValues.AddRange(cashTotals.CashTotals.HoppersInfo.CoinsInfo
                                    .Select(r => new WithdrawalCashModel()
                                    {
                                        CashType = CashType.Coins,
                                        Denomination = r.Value,
                                        RecyclerQuantity = r.Count
                                    }).ToList());

            return recyclerValues;
        }

        public bool IsConnected => this.ZeusConnectionService.IsConnected;
    }


}
