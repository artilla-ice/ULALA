using System;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;
using ULALA.Domain.Contracts.Data;
using ULALA.Domain.Data;

namespace ACS.Infrastructure.Data
{
    public class TableRepository<T> : ITableRepository<T> where T : TableEntityBase, new()
	{
		public TableRepository(string connectionString, string tableNameSuffix = null)
		{
			m_tableName = typeof(T).Name.ToLowerInvariant();

            if (tableNameSuffix != null)
                m_tableName += tableNameSuffix.ToLowerInvariant();

			Initialize(connectionString);
		}

		public void AddEntity(T entity)
		{
            GetBatchOperation().Insert(entity);
		}

		public void UpdateEntity(T entity)
		{
            GetBatchOperation().Replace(entity);
		}

		/// <summary>
		/// Adds or updates an entity to the table
		/// </summary>
		/// <param name="entity"></param>
		public void AddOrReplace(T entity)
		{
            GetBatchOperation().InsertOrReplace(entity);
		}

		public void DeleteEntity(T entity)
		{
            GetBatchOperation().Delete(entity);
		}
		public void DeleteEntitiesByPartitionKey(object partitionId)
		{
			var entities = QueryEntitiesByPartition(partitionId);
			var batchOp = GetBatchOperation();
			foreach (var entity in entities)
				batchOp.Delete(entity);
		}

		public void DeleteEntity(object partitionId, object rowId)
		{
			var entity = FindEntity(partitionId, rowId);
			if ( entity != null )
                GetBatchOperation().Delete(entity);
        }

		public T FindEntity(object partitionId, object RowId)
		{
			var pk = partitionId.ToString();
			var rk = RowId.ToString();

            return (T)m_table.Execute(TableOperation.Retrieve<T>(pk, rk)).Result;
		}

        public T FindEntityByRowKey(object RowId)
        {
            var condition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, RowId.ToString());
            TableQuery<T> tableQuery = new TableQuery<T>().Where(condition);

            return m_table.ExecuteQuery(tableQuery).SingleOrDefault();
        }

        public T FindEntity(string composedKey)
		{
			var keys = TableEntityBase.DecomposeKey(composedKey);

			var k1 = keys.Item1;
			var k2 = keys.Item2;

            return FindEntity(k1, k2);
		}

		public IQueryable<T> QueryEntitiesByPartition(object partitionId)
		{

			var pk = partitionId.ToString();

            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pk);
            TableQuery<T> tableQuery = new TableQuery<T>().Where(condition);

            return m_table.ExecuteQuery(tableQuery).AsQueryable<T>();
		}

        public IQueryable<T> QueryAllEntities()
        {
            return m_table.ExecuteQuery(new TableQuery<T>()).AsQueryable<T>();
        }

		public void Commit()
		{
            if (m_queuedOperations != null)
            {
                m_table.ExecuteBatch(m_queuedOperations);
                m_queuedOperations = null;
            }
                
		}

		public void DeleteTable()
		{
			m_table.DeleteIfExists();
		}

		//--------------------------------------------------------------------------------------------------------------------------------------
		// IDisposable
		//--------------------------------------------------------------------------------------------------------------------------------------
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TableRepository()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool bDisposing)
		{
			if ( bDisposing )
			{
				m_queuedOperations = null;
				m_tableClient	= null;
				m_table			= null;
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------------
		// PRIVATE MEMBERS
		//--------------------------------------------------------------------------------------------------------------------------------------

		private CloudTableClient GetStorageTable(string connectionString)
		{
			// Reference storage account from connection string. 
			var storageAccount = CloudStorageAccount.Parse(connectionString);

			// Create Table service client.
			var tableClient = storageAccount.CreateCloudTableClient();
			
			return tableClient;
		}

		private void Initialize(string connectionString)
		{
			m_tableClient = GetStorageTable(connectionString);

			m_table = m_tableClient.GetTableReference(m_tableName);
			m_table.CreateIfNotExists();

		}

        private TableBatchOperation GetBatchOperation()
        {
            if (m_queuedOperations == null)
                m_queuedOperations = new TableBatchOperation();

            return m_queuedOperations;
        }

        private TableBatchOperation m_queuedOperations = null;
		private	CloudTableClient m_tableClient;
		private	CloudTable m_table;
		private readonly string m_tableName;
	}
}
