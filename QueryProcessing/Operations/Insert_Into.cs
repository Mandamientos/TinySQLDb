using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Insert_Into
    {
        public static (OperationStatus, string) execute(string tableName, List<string> Inserts)
        {
            string dbName = SQLProcessor.selectedDB;
            if (string.IsNullOrEmpty(dbName))
            {
                return (OperationStatus.Error, "Unknown error.");
            }
            Console.WriteLine(AddInsert.checkExistence(dbName, tableName));
            Console.WriteLine(ValidateInserts(Inserts, GetTableColumns.GetTableNameColumns(dbName, tableName)));
            if (AddInsert.checkExistence(dbName, tableName) && ValidateInserts(Inserts, GetTableColumns.GetTableNameColumns(dbName, tableName)))
            {
                AddInsert.execute(dbName, tableName, Inserts);
                return (OperationStatus.Success, "Data has been inserted successfully.");
            }
            return (OperationStatus.Error, "Data could not be inserted.");
        }

        public static bool ValidateInserts(List<string> Inserts, List<List<string>> cols)
        {
            // Check if sizes are equal
            if (Inserts.Count != cols.Count)
            {
                return false;
            }

            for (int i = 0; i < Inserts.Count; i++)
            {
                Console.WriteLine(i);
                string colType = cols[i][1];
                string insertValue = Inserts[i];

                if (colType == "INTEGER")
                {
                    Console.WriteLine("int "+ insertValue);
                    Console.WriteLine();
                    // Try to parse as Integer
                    if (!int.TryParse(insertValue, out _))
                    {
                        return false; // Parsing failed
                    }
                }
                else if (colType == "DATETIME")
                {
                    Console.WriteLine("datetime ", insertValue);
                    // Check if it matches "yyyy-MM-dd HH:mm:ss" format
                    if (!DateTime.TryParseExact(insertValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        return false; // Parsing failed
                    }
                }
                else if (colType.Contains("VARCHAR"))
                {
                    Console.WriteLine("varchar ", insertValue);
                    // Extract the max length from VARCHAR(x)
                    Match match = Regex.Match(colType, @"VARCHAR\((\d+)\)");
                    if (match.Success)
                    {
                        int maxLength = int.Parse(match.Groups[1].Value);

                        // Check if the length of the insert value is within the allowed size
                        if (insertValue.Length > maxLength)
                        {
                            return false; // Exceeds VARCHAR length
                        }
                    }
                    else
                    {
                        return false; // Invalid VARCHAR definition
                    }
                }
            }

            return true; // All checks passed
        }
    }

}
