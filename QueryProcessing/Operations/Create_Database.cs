using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    public class Create_Database
    {
        private static string path = @"C:\TinySQLDb\Databases";
        public static (OperationStatus, string) execute(string sentence)
        {
            string dbName = sentence.Substring("CREATE DATABASE ".Length).Trim();
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "Illegal database name.");
            } else
            {
                foreach(char character in dbName)
                {
                    if(!char.IsLetterOrDigit(character))
                    {
                        return (OperationStatus.Error, "Illegal database name.");
                    }
                }

                string dbPath = Path.Combine(path, dbName);
                if(!AddDatabase.checkExistence(dbName))
                {
                    Directory.CreateDirectory(dbPath);
                    AddDatabase.execute(dbName);
                    return (OperationStatus.Success, $"Database '{dbName}' created successfully.");
                } else
                {
                    return (OperationStatus.Error, "Unknown error.");
                }

            }
        }
    }
}
