using System;
using System.Threading.Tasks;

namespace ULALA.Domain.Contracts.Data
{
    public interface IDataRepository : IDisposable
    {
        object  Context { get; }

        void	Commit();
        Task<int> CommitAsync();
        void    DetachEntry(object entry);
	}
}
