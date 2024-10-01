using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Insert_Into
    {
        public static (OperationStatus, string) execute(string tableName, List<string> Inserts)
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "Unknown error.");
            }
            else if (AddInsert.checkExistence(dbName, tableName))
            {
                Console.WriteLine(tableName);
                Console.WriteLine(Inserts.Count);
                AddInsert.execute(dbName, tableName, Inserts);
                return (OperationStatus.Success, "Data has been inserted successfully.");
            }
            return (OperationStatus.Error, "Unknown error.");
        }
    }
}
