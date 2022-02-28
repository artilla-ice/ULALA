
using ULALA.Domain.Contracts.Data;
using ULALA.Domain.Contracts.Models;

namespace ULALA.Domain.Data
{
    public class LogsRepository : DataRepositoryBase<CoreDataRepositoryContext>, ILogsRepository
    {
        public LogsRepository(CoreDataRepositoryContext ctx, bool bDisposeCtx) : base(ctx, bDisposeCtx)
        {
        }

        public void AddInfoLog(ActionsLog log)
        {
            ValidateDataContext();

            this.DataContext.ActionsLogs.Add(log);
        }
    }
}
