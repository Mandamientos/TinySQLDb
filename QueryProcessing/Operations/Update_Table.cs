using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Update_Table
    {

        public static (OperationStatus, string) execute(List<string> update) 
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "No DB selected.");
            }
            else if (AddInsert.checkExistence(dbName, update[0]))
            {
                //extrae las columnas de la tabla
                List<string> cols = GetTableColumns.GetTableNameColumns(dbName, update[0]);
                //valida que la columna es parte de la lista
                if (cols.Contains(update[1]) && update.Count == 3)
                {
                    UpdateTable.execute(dbName, update, cols.IndexOf(update[1]));
                    return (OperationStatus.Success, "Data has been updated successfully.");
                }
                else if (cols.Contains(update[1]) && cols.Contains(update[3])) 
                {
                    UpdateTable.execute(dbName, update, cols.IndexOf(update[1]), cols.IndexOf(update[3]));
                    return (OperationStatus.Success, "Data has been updated successfully.");
                }
            }
            return (OperationStatus.Error, $"No table {update[0]} in DB {dbName}.");
        }



    }
}
