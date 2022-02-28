using System;
using Microsoft.Azure.Cosmos.Table;

namespace ULALA.Domain.Data
{
    public class TableEntityBase : TableEntity
    {
        public TableEntityBase()
        {

        }

        public TableEntityBase(string parititonKey, string rowkey)
        {
            this.PartitionKey = parititonKey;
            this.RowKey = rowkey;
        }

        public string ComposedKey()
        {
            return String.Format("{0}|{1}", this.PartitionKey, this.RowKey);
        }


        public static Tuple<string, string> DecomposeKey(string composedKey)
        {
            var keys = composedKey.Split('|');

            if (keys.Length != 2)
                throw new ArgumentException("Invalid table entity composed key, the composed key must contain two strings separated by the | symbol");

            return new Tuple<string, string>(keys[0], keys[1]);
        }
    }
}
