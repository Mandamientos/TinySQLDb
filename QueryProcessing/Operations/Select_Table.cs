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
            if (selectQuery == null) return (OperationStatus.Error, "Syntax error.");
            if (string.IsNullOrEmpty(SQLProcessor.selectedDB)) return (OperationStatus.Error, "No database selected.");
            if (DropTable.isItEmpty(selectQuery.tableName, SQLProcessor.selectedDB)) return (OperationStatus.Error, "Empty table.");
            if (!string.Equals(selectQuery.columns[0], "*"))
            {
                if ((!AddColumns.checkExistance(SQLProcessor.selectedDB, selectQuery.columns)) && AddTable.checkExistence(SQLProcessor.selectedDB, selectQuery.tableName)) return (OperationStatus.Error, "The specified columns or table do not exist in the system files.");
                else
                {
                    return handleTable(selectQuery);
                }
            } else
            {
                if (AddTable.checkExistence(SQLProcessor.selectedDB, selectQuery.tableName))
                {
                    return handleTable(selectQuery);
                }
                else return (OperationStatus.Error, "Selected table doesn't exist.");
            }
        }

        public static (OperationStatus Success, string response) handleTable (SelectModel selectQuery)
        {
            List<List<string>> dataFormatter = GetTableColumns.GetTableNameColumns(SQLProcessor.selectedDB, selectQuery.tableName);

            List<Object[]> rawData = GetTableData.getData(SQLProcessor.selectedDB, selectQuery.tableName, dataFormatter);

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();


            if (!string.IsNullOrEmpty(selectQuery.whereColumn))
            {
                rawData = GetTableData.whereClause(selectQuery.whereColumn, selectQuery.whereComparator, selectQuery.whereValue, rawData, dataFormatter);
                if (Equals(rawData, null) || !rawData.Any()) return (OperationStatus.Error, "No records were found matching the specified WHERE clauses.");
            }

            foreach (var row in rawData)
            {
                Dictionary<string, object> rowDict = new Dictionary<string, object>();

                for (int i = 0; i < dataFormatter.Count; i++)
                {
                    rowDict.Add($"{dataFormatter[i][0]}", row[i]);
                }

                data.Add(rowDict);
            }

            if (!string.Equals(selectQuery.columns[0], "*"))
            {
                data = delUnnecessaryKeys(data, selectQuery.columns);
            }

            string response = JsonSerializer.Serialize(data);
            Console.WriteLine(response);

            return (OperationStatus.Success, response);
        }

        public static List<Dictionary<string, object>> delUnnecessaryKeys (List<Dictionary<string, object>> data, List<string> requiredCols)
        {
            foreach (var rowDict in data) {
                List<string> keysToDelete = new List<string>();

                foreach (var key in rowDict.Keys)
                {
                    if(!requiredCols.Contains(key)) keysToDelete.Add(key);
                }

                foreach (var key in keysToDelete) {
                    rowDict.Remove(key);
                }
            }

            return data;
        }
    }
}
