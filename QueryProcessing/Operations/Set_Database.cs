using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    public class Set_Database
    {
        public static OperationStatus execute(string sentence) {
            string dbName = sentence.Substring("SET DATABASE ".Length).Trim();
            Console.WriteLine(dbName);

            if(string.IsNullOrEmpty(dbName))
            {
                return OperationStatus.Error;
            } 
            else if (AddDatabase.checkExistence(dbName))
            {
                SQLProcessor.selectedDB = dbName;
                return OperationStatus.Success;
            }

            return OperationStatus.Error;
        }
    }
}
