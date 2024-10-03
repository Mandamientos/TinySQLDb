using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class ColumnModel
    {
        public string colName { get; set; }
        public string colType { get; set; }
    }
    public class GetTableData
    {
        public static List<Object[]> getData(string dbName, string tableName, List<List<string>> modelFormatter)
        {
            string dbTablePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table";

            List<ColumnModel> formatPattern = new List<ColumnModel>();

            for (int i = 0; i < modelFormatter.Count; i++)
            {
                var column = new ColumnModel
                {
                    colName = modelFormatter[i][0],
                    colType = modelFormatter[i][1]
                };

                formatPattern.Add(column);
            }

            List<Object[]> dataMatrix = new List<Object[]>();

            using (BinaryReader reader = new BinaryReader(File.Open(dbTablePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentString = reader.ReadString();
                    if(!string.IsNullOrEmpty(currentString))
                    {
                        string[] rawRow = currentString.Split(',');
                        object[] row = new object[formatPattern.Count];

                        for (int i = 0; i < formatPattern.Count; i++) {
                            var columnValues = formatPattern[i];
                            string value = rawRow[i];

                            row[i] = convertData(value, columnValues.colType);
                        }
                        dataMatrix.Add(row);
                    }
                }
            }

            return dataMatrix;
        }

        public static object convertData(string data, string dataType)
        {
            if (dataType.StartsWith("VARCHAR")) return data;

            switch (dataType) {
                case "INTEGER":
                    return int.Parse(data);
                case "DOUBLE":
                    return double.Parse(data);
                case "DATETIME":
                    return DateTime.Parse(data);
                default:
                    throw new NotImplementedException();
            }
        }

        public static List<object[]> whereClause(string whereColName, string whereOperator, object whereValue, List<object[]> rawData, List<List<string>> tableFormat)
        {
            int index = tableFormat.FindIndex(sublist => sublist[0] == whereColName);
            if (Equals(index, -1)) return null;

            if (whereValue.GetType() != rawData[0][index].GetType()) return null;

            for (int i = rawData.Count - 1; i >= 0 ; i--)
            {
                if (whereValue.GetType() == typeof(string))
                {
                    if (!StringCompare(whereOperator, (string)whereValue, (string)rawData[i][index]))
                    {
                        rawData.RemoveAt(i);
                    }
                } else if (whereValue.GetType() == typeof(int))
                {
                    if (!IntegerCompare(whereOperator, (int)whereValue, (int)rawData[i][index]))
                    {
                        rawData.RemoveAt(i);
                    }
                } else if (whereValue.GetType() == typeof(DateTime))
                {
                    if (!DateTimeCompare(whereOperator, (DateTime)whereValue, (DateTime)rawData[i][index]))
                    {
                        rawData.RemoveAt(i);
                    }
                }
                else if (whereValue.GetType() == typeof(double))
                {
                    if (!DoubleCompare(whereOperator, (double)whereValue, (double)rawData[i][index]))
                    {
                        rawData.RemoveAt(i);
                    }
                }

            }

            return rawData;
        }

        public static bool StringCompare(string whereOperator, string whereValue, string tableValue)
        {
            Console.WriteLine($"{whereValue} {whereOperator} {tableValue}");
            switch (whereOperator)
            {
                case "==":
                    if (Equals(whereValue, tableValue)) return true;
                    else return false;
                case "LIKE":
                    if (tableValue.Contains(whereValue)) return true;
                    else return false;
                case "NOT":
                    if (!Equals(whereValue, tableValue)) return true;
                    else return false;
                case ">":
                    if (string.Compare(whereValue, tableValue, StringComparison.OrdinalIgnoreCase) < 0) return true;
                    else return false;
                case "<":
                    if (string.Compare(whereValue, tableValue, StringComparison.OrdinalIgnoreCase) > 0) return true;
                    else return false;
                default:
                    return false;
            }
        }

        public static bool IntegerCompare(string whereOperator, int whereValue, int tableValue)
        {
            Console.WriteLine($"{whereValue} {whereOperator} {tableValue}");
            switch (whereOperator)
            {
                case "==":
                    if (Equals(whereValue, tableValue)) return true;
                    else return false;
                case "NOT":
                    if (!Equals(whereValue, tableValue)) return true;
                    else return false;
                case ">":
                    if (whereValue < tableValue) return true;
                    else return false;
                case "<":
                    if (whereValue > tableValue) return true;
                    else return false;
                default:
                    return false;
            }
        }
        public static bool DoubleCompare(string whereOperator, double whereValue, double tableValue)
        {
            Console.WriteLine($"{whereValue} {whereOperator} {tableValue}");
            switch (whereOperator)
            {
                case "==":
                    if (Equals(whereValue, tableValue)) return true;
                    else return false;
                case "NOT":
                    if (!Equals(whereValue, tableValue)) return true;
                    else return false;
                case ">":
                    if (whereValue < tableValue) return true;
                    else return false;
                case "<":
                    if (whereValue > tableValue) return true;
                    else return false;
                default:
                    return false;
            }
        }

        public static bool DateTimeCompare(string whereOperator, DateTime whereValue, DateTime tableValue)
        {
            Console.WriteLine($"{whereValue} {whereOperator} {tableValue}");
            switch (whereOperator)
            {
                case "==":
                    if (Equals(whereValue, tableValue)) return true;
                    else return false;
                case "NOT":
                    if (!Equals(whereValue, tableValue)) return true;
                    else return false;
                case ">":
                    if (whereValue < tableValue) return true;
                    else return false;
                case "<":
                    if (whereValue > tableValue) return true;
                    else return false;
                default:
                    return false;
            }
        }

    }
}
