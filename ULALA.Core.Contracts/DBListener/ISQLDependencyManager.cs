using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Base;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;

namespace ULALA.Core.Contracts.Zeus
{
    public interface ISQLDependencyManager : IInitializableManager
    {
        void StartListening(Object state);
    }
}
