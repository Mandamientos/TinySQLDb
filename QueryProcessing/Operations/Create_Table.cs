using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Create_Table
    {

        public static (OperationStatus, string) execute(string tableName, Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)> CreateColumns)
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "Unknown error.");
            }
            else if (!AddTable.checkExistence(dbName, tableName))
            {
                AddTable.execute(dbName, tableName);
                AddColumns.execute(dbName, tableName, CreateColumns);
                return (OperationStatus.Success, "Table successfully created.");
            }
            return (OperationStatus.Error, "Unknown error.");
        }


    }
}
