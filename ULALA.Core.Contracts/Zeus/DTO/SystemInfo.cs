using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ULALA.UI.Core.MVVM;

namespace ULALA.Core.Contracts.Zeus.DTO
{
    public enum SystemInfoResultCode
    {
        UnknownOperationError           = 100,
        NotEnoughCashError              = 101,
        BillRecyclerNotRespondingError  = 102,
        CoinValidatorNotRespondingError = 103,
        BillJammedError                 = 104, 
        CoinStuckError                  = 105,
        StackerFullError                = 106,
        StackerMissingError             = 107,
        RecyclerFailedError             = 108,
        OpenDoorWarning                 = 111,
    }
}
