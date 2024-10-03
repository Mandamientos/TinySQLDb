using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                List<List<string>> cols = GetTableColumns.GetTableNameColumns(dbName, update[0]);
                //valida que la columna es parte de la lista
                int index = getIndex(cols, update[1]);
                if (index != -1 && update.Count == 3)
                {
                    if (validateType(cols, update[1], update[2]))
                    {
                        UpdateTable.execute(dbName, update, index);
                        return (OperationStatus.Success, "Data has been updated successfully.");
                    }
                    else
                    {
                        return (OperationStatus.Error, $"Value {update[2]} incorrect for column {update[1]}.");
                    }
                }
                else if (index != -1 && getIndex(cols, update[3]) != -1)
                {
                    if (validateType(cols, update[1], update[2]) && validateType(cols, update[3], update[4]))
                    {
                        UpdateTable.execute(dbName, update, index, getIndex(cols, update[3]));
                        return (OperationStatus.Success, "Data has been updated successfully.");
                    }
                    else
                    {
                        return (OperationStatus.Error, $"Value {update[2]} incorrect for column {update[1]}.");
                    }
                }
            }
            return (OperationStatus.Error, $"No table {update[0]} in DB {dbName}.");
        }

        private static int getIndex(List<List<string>> cols, string value) 
        {
            for (int i = 0; i < cols.Count; i++) 
            {
                if (cols[i].Contains(value)) 
                {
                    return i;
                }
            }
            return -1;
        }

        private static bool validateType(List<List<string>> cols, string table, string value) 
        {
            int index = getIndex(cols, table);
            if (index != -1) 
            {
                string type = cols[index][1];
                switch (type) 
                {
                    case "INTEGER":
                        if (int.TryParse(value, out _)) 
                        {
                            return true;
                        }
                        break;
                    case "DATETIME":
                        return true;
                    default:
                        if (type.Contains("VARCHAR"))
                        {
                            string pattern = @"\((\d+)\)";
                            Match match = Regex.Match(type, pattern);
                            string v = match.Groups[1].Value;
                            int i = int.Parse(v);
                            if (value.Length <= i) 
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

    }
}
