using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Logger;
using ULALA.Domain.Contracts.Data;
using Unity;

namespace ULALA.Core.Logger
{
    public class Logger : ILogger
    {
        [Dependency]
        public IDomainRepositoryFactory RepositoryFactory { get; set; }

        public void WriteEvent()
        {
            try
            {
                using (var repo = this.RepositoryFactory.CreateRepository<ILogsRepository>())
                {
                    repo.AddInfoLog(new Domain.Contracts.Models.ActionsLog()
                    {
                        Id = Guid.NewGuid(),
                        Date = DateTime.Today
                    });

                    repo.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void WriteError()
        {

        }

        public void WriteWarning()
        {

        }
    }
}
