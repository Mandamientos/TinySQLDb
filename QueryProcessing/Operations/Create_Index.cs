using StoreSystem.CatalogOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Text.RegularExpressions;

namespace QueryProcessing.Operations
{
    internal class Create_Index
    {

        private static string indexName;
        private static string tableName;
        private static string columnName;
        private static string indexType;

        public static (OperationStatus, string) execute(string sql)
        {
            setValues(sql);
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "No se ha seleccion una base de datos.");
            }
            else if (AddInsert.checkExistence(dbName, tableName))
            {
                if (AddIndex.checkExistance(dbName, tableName, indexName, columnName, indexType)){ Console.WriteLine("checked"); }
                if (AddIndex.canBeIndex(dbName, tableName, columnName)) { Console.WriteLine("can be"); }
                if ((!AddIndex.checkExistance(dbName, tableName, columnName, indexName , indexType)) && AddIndex.canBeIndex(dbName, tableName, columnName))
                {
                    AddIndex.execute(dbName, tableName, indexName, columnName, indexType);
                    return (OperationStatus.Success, "Index creado");
                }
                else
                    return (OperationStatus.Error, "No se puede formar indices en columnas que no sean UNIQUE o PRIMARY KEY.");
            }
            return (OperationStatus.Error, "Unknown error.");
        }

        private static void setValues(string sql)
        {
            var pattern = @"CREATE INDEX\s+(?<indexName>\w+)\s+ON\s+(?<tableName>\w+)\s*\((?<columnName>\w+)\)\s+OF\s+TYPE\s+(?<indexType>BTREE|BST)\s*";

            // Use Regex to match the pattern
            var match = Regex.Match(sql, pattern, RegexOptions.IgnoreCase);

            indexName = match.Groups["indexName"].Value.Trim();
            tableName = match.Groups["tableName"].Value.Trim();
            columnName = match.Groups["columnName"].Value.Trim();
            indexType = match.Groups["indexType"].Value.Trim();

            Console.WriteLine($"Index: {indexName}, Table: {tableName}, Column: {columnName}, Type: {indexType}");

        }
    }
}
