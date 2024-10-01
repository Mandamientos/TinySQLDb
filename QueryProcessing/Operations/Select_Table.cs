using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using QueryProcessing.Models;
using Entities;
using StoreSystem.CatalogOperations;
using System.Text.Json;

namespace QueryProcessing.Operations
{
    internal class Select_Table()
    {
        public static (OperationStatus, string) execute(SelectModel selectQuery)
        {
            if (string.IsNullOrEmpty(SQLProcessor.selectedDB)) return (OperationStatus.Error, "No database selected.");
            if (DropTable.isItEmpty(selectQuery.tableName, SQLProcessor.selectedDB)) return (OperationStatus.Error, "Empty table.");
            if (selectQuery == null) return (OperationStatus.Error, "Unknown error.");
            if (!string.Equals(selectQuery.columns[0], "*"))
            {
                if ((!AddColumns.checkExistance(SQLProcessor.selectedDB, selectQuery.columns)) && AddTable.checkExistence(SQLProcessor.selectedDB, selectQuery.tableName)) return (OperationStatus.Error, "The specified columns or table do not exist in the system files.");
            } else
            {
                selectQuery.columns = GetTableColumns.GetTableNameColumns(SQLProcessor.selectedDB, selectQuery.tableName);
            }

            List<List<String>> rawData = GetTableData.getData(SQLProcessor.selectedDB, selectQuery.tableName);

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

            foreach(var row in rawData)
            {
                Dictionary<string, object> rowDict = new Dictionary<string, object>();

                for (int i = 0; i < selectQuery.columns.Count; i++)
                {
                    rowDict.Add($"{selectQuery.columns[i]}", row[i]);
                }

                data.Add(rowDict);
            }

            string response = JsonSerializer.Serialize(data);

            return (OperationStatus.Success, response);
        }
    }
}
