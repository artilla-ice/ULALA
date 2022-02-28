
using System;
using System.Data.Common;
using ULALA.Domain.Contracts.Utility;
using ULALA.Domain.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace ULALA.Domain.Data
{
    public class CoreDataRepositoryContext : DbContext
    {
        public CoreDataRepositoryContext(string connectionString) 
        {
            
        }

        public static CoreDataRepositoryContext CreateDbContext(object connStringOrConnection)
        {
            string connectionString = connStringOrConnection as String;
            if (connectionString != null)
                return new CoreDataRepositoryContext(connectionString);
            else
            {
                //TODO: throw exception
                return null;
            }
        }

        public DbSet<ActionsLog> ActionsLogs { get; set; }
        private const int CommandsTimeout = 30 * 60;
    }
}

