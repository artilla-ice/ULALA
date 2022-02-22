using System;
using System.Data.Entity;
using System.Threading.Tasks;
using ULALA.Domain.Contracts.Data;

namespace ULALA.Domain.Data
{
    public class DataRepositoryBase<T> : IDataRepository where T : DbContext
	{
		public DataRepositoryBase(T ctx, bool bDisposeContext)
		{
			m_bDisposeContext	= bDisposeContext;
			this.DataContext	= ctx;
        }

        public object Context
        {
            get
            {
                return this.DataContext;
            }
        }

        public void Commit()
		{
			this.DataContext.SaveChanges();
        }

		public Task<int> CommitAsync()
		{
			return this.DataContext.SaveChangesAsync();
		}

		public void DetachEntry(object entry)
        {
            this.DataContext.Entry(entry).State = EntityState.Detached;
        }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DataRepositoryBase()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool bDisposing)
		{
			if ( bDisposing )
			{
				if ( this.DataContext != null && m_bDisposeContext )
					this.DataContext.Dispose();

				this.DataContext = null;
			}
		}

		protected void ValidateDataContext()
		{
			if ( this.DataContext == null )
				throw new InvalidOperationException("The repository data context is null");
		}

		protected T DataContext { get; set; }
		private  bool m_bDisposeContext;
	}
}
