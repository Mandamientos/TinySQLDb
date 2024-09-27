using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Create_Table
    {
        private static string path = @"C:\TinySQLDb\SystemCatalog\SystemTables.dat";

        public static OperationStatus execute(string tableName, Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)> CreateColumns)
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return OperationStatus.Error;
            }
            else if (!AddTable.checkExistence(dbName, tableName))
            {
                AddTable.execute(dbName, tableName);
                AddColumns.execute(dbName, tableName, CreateColumns.First().Key, CreateColumns.First().Value.DataType, CreateColumns.First().Value.IsNullable, CreateColumns.First().Value.Constraints.First());
                return OperationStatus.Success;
            }
            return OperationStatus.Error;
        }

    }
}
