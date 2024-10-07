using StoreSystem.CatalogOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Delete_From
    {
        public static (OperationStatus, string) execute(List<string> delete)
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "No DB selected.");
            }
            else
            {
                DeleteFrom.execute(delete);
            }
        }
}
