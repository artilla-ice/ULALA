using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ULALA.Domain.Contracts.Data;

namespace ULALA.Domain.Data
{
    public class DomainRepositoryFactory : IDomainRepositoryFactory
	{
		public T CreateRepository<T>(params object[] ctx) where T : IDataRepository
		{
			Func<object, object, IDataRepository> factoryFunc = null;

			if ( FactoryMap.TryGetValue(typeof(T), out factoryFunc) )
				return (T)factoryFunc(GetConnectionString(), (ctx != null && ctx.Length > 0) ? ctx[0] : null);
			else
				throw new ArgumentException("Domain data repository is not registered");
		}

		private static Dictionary<Type, Func<object, object, IDataRepository>> FactoryMap = new Dictionary<Type,Func<object, object,IDataRepository>>()
		{
			{typeof(ILogsRepository), (connstring, ctx)=> 
					{ return new LogsRepository(ctx != null ? (CoreDataRepositoryContext)ctx : 
						CoreDataRepositoryContext.CreateDbContext(connstring), ctx == null);  }},
        };

		private string GetConnectionString()
		{
			var sqlConnectionSB = new SqlConnectionStringBuilder();

			sqlConnectionSB.DataSource = "LAPTOP-S5FBM5U2";//TODO: cambiar a manager de configuracion
			sqlConnectionSB.InitialCatalog = "ULALA";//"ACSCoreDB56";  

			sqlConnectionSB.UserID = "sa";
			sqlConnectionSB.Password = "gttbhr";
			sqlConnectionSB.IntegratedSecurity = false;

			sqlConnectionSB.Encrypt = true;
			sqlConnectionSB.ConnectTimeout = 30;
			sqlConnectionSB.TrustServerCertificate = false;
			sqlConnectionSB.MultipleActiveResultSets = true;

			return sqlConnectionSB.ToString();
		}
	}
}
 