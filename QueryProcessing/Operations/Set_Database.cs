using System;
using System.Collections.Generic;
using System.IO;
using StoreSystem.CatalogOperations;
using Entities;
using QueryProcessing.SQLParser;

namespace QueryProcessing.Operations
{
    public class Set_Database
    {
        public static (OperationStatus, string) execute(string sentence)
        {
            string dbName = sentence.Substring("SET DATABASE ".Length).Trim();
            Console.WriteLine(dbName);

            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "Illegal database name.");
            }
            else if (AddDatabase.checkExistence(dbName))
            {
                SQLProcessor.selectedDB = dbName;

                // Load indexes and initialize trees for the selected database
                Create_Index.cargarIndexes();

                return (OperationStatus.Success, $"Database '{dbName}' established successfully.");
            }

            return (OperationStatus.Error, "Seems like that database doesn't exist.");
        }

    }
}
