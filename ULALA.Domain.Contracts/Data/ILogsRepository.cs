using System;
using System.Collections.Generic;
using System.Text;
using ULALA.Domain.Contracts.Models;

namespace ULALA.Domain.Contracts.Data
{
    public interface ILogsRepository : IDataRepository
    {
        void AddInfoLog(ActionsLog log);
    }
}
