
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using ULALA.Domain.Contracts.Utility;
using ULALA.Domain.Contracts.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ULALA.Domain.Data
{
    public class CoreDataRepositoryContext : DbContext
    {
        public CoreDataRepositoryContext(string connectionString) : base(connectionString)
        {
            //var adapter = ((IObjectContextAdapter)this).ObjectContext;
            //var objectContext = adapter.ObjectContext;
            //objectContext.CommandTimeout = CommandsTimeout; // value in seconds

            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;

            //((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
            //(sender, e) => DateTimeKindAttribute.Apply(e.Entity);
        }

        //public CoreDataRepositoryContext(DbConnection conn, bool contextOwnsConnection = false)
        //    : base(conn, contextOwnsConnection)
        //{
        //    //this.Configuration.ProxyCreationEnabled = false;
        //    //this.Configuration.LazyLoadingEnabled = false;

        //    //((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
        //    //(sender, e) => DateTimeKindAttribute.Apply(e.Entity);
        //}

        public static CoreDataRepositoryContext CreateDbContext(object connStringOrConnection)
        {
            DbConnection connection = connStringOrConnection as DbConnection;

            //if (connection != null)
            //    return new CoreDataRepositoryContext(connection, true);
            //else
            //{
                string connectionString = connStringOrConnection as String;
                if (connectionString != null)
                    return new CoreDataRepositoryContext(connectionString);
                else
                {
                    //TODO: throw exception
                    return null;
                }
            //}
        }

        public DbSet<ActionsLog> ActionsLogs { get; set; }

        private const int CommandsTimeout = 30 * 60;
    }
}

