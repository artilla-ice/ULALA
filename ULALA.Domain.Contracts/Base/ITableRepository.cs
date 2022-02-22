using System;
using System.Linq;

namespace ULALA.Domain.Contracts.Data
{
    public interface ITableRepository<T> : IDisposable
	{
		void AddEntity(T entity);
		void UpdateEntity(T entity);
		void DeleteEntity(T entity);
		void AddOrReplace(T entity);
		void DeleteEntity(object partitionId, object rowId);
		T FindEntity(object partitionId, object RowId);
		T FindEntity(string composedKey);
		IQueryable<T> QueryEntitiesByPartition(object partitionId);
		void Commit();
		void DeleteTable();
	}
}
