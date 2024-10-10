using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class DeleteFrom
    {
        private const string tabDBPath = @"C:\TinySQLDb\";

        public static void execute(List<string> delete)
        {
            // Construct the path to the database table file
            string path = tabDBPath + $@"Databases\{delete[0]}\{delete[1]}.table";

            if (delete.Count == 2)
            {
                // If only the database and table name are provided, delete all data
                File.WriteAllText(path, string.Empty); // Clear the file content
                Console.WriteLine("All data has been deleted successfully.");
            }
            else if (delete.Count > 2)
            {
                // Read the file contents
                byte[] fileBytes = File.ReadAllBytes(path);
                string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);
                var lines = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Convert the column index from delete[4]
                if (int.TryParse(delete[4], out int columnIndex))
                {
                    string comparisonValue = delete[2].Trim('\''); // The value to compare with
                    string columnType = delete[3];                  // The type of the column
                    string operatorSymbol = delete[5];              // The operator (e.g., =, !=, >, <)

                    // Extract VARCHAR length if applicable
                    int varcharLength = 0;
                    if (columnType.StartsWith("VARCHAR", StringComparison.OrdinalIgnoreCase))
                    {
                        var lengthPart = columnType.Substring("VARCHAR".Length).Trim('(', ')');
                        int.TryParse(lengthPart, out varcharLength);
                    }

                    // Loop through lines to find and delete matching rows
                    for (int i = lines.Count - 1; i >= 0; i--) // Iterate in reverse to safely remove items
                    {
                        // Split the line into columns
                        var columns = lines[i].Split(',');

                        // Check if the index is valid
                        if (columnIndex < columns.Length)
                        {
                            // Get the value at the specified column index
                            string columnValue = columns[columnIndex].Trim('\''); // Trim quotes for comparison

                            // Check if the condition is met based on the column type and operator
                            if (IsConditionMet(columnValue, comparisonValue, operatorSymbol, columnType, varcharLength))
                            {
                                // Remove the line if the condition is met
                                lines.RemoveAt(i);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Column index {columnIndex} is not valid for line {i}.");
                        }
                    }

                    // Write the modified content back to the file
                    string modifiedContent = string.Join("\n", lines);
                    byte[] modifiedBytes = System.Text.Encoding.UTF8.GetBytes(modifiedContent);

                    // Write the binary file with the updated lines
                    using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
                    {
                        writer.Write(modifiedBytes);
                    }

                    Console.WriteLine("Matching data has been deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Invalid column index: {delete[4]}");
                }
            }
            else
            {
                Console.WriteLine("Invalid delete command.");
            }
        }

        // Method to check if the condition between columnValue and comparisonValue meets the operator criteria
        private static bool IsConditionMet(string columnValue, string comparisonValue, string operatorSymbol, string columnType, int varcharLength)
        {
            switch (columnType.ToUpper())
            {
                case "INTEGER":
                    // Try to parse to integer and compare
                    if (int.TryParse(columnValue, out int columnIntValue) && int.TryParse(comparisonValue, out int comparisonIntValue))
                    {
                        return CompareInt(columnIntValue, comparisonIntValue, operatorSymbol);
                    }
                    return false;

                case string ct when ct.StartsWith("VARCHAR", StringComparison.OrdinalIgnoreCase):
                    // Check length before comparison
                    if (comparisonValue.Length <= varcharLength)
                    {
                        return CompareString(columnValue, comparisonValue, operatorSymbol);
                    }
                    return false;

                case "DATETIME":
                    // Compare as DateTime
                    return CompareDateTime(columnValue, comparisonValue, operatorSymbol);

                default:
                    throw new InvalidOperationException($"Unsupported column type: {columnType}");
            }
        }

        // Method for integer comparison based on the operator
        private static bool CompareInt(int columnIntValue, int comparisonIntValue, string operatorSymbol)
        {
            return operatorSymbol switch
            {
                "=" => columnIntValue == comparisonIntValue,
                "!=" => columnIntValue != comparisonIntValue,
                ">" => columnIntValue > comparisonIntValue,
                "<" => columnIntValue < comparisonIntValue,
                ">=" => columnIntValue >= comparisonIntValue,
                "<=" => columnIntValue <= comparisonIntValue,
                _ => throw new InvalidOperationException($"Invalid operator for integer comparison: {operatorSymbol}"),
            };
        }

        // Method for string comparison based on the operator
        private static bool CompareString(string columnValue, string comparisonValue, string operatorSymbol)
        {
            return operatorSymbol switch
            {
                "=" => columnValue.Equals(comparisonValue),
                "!=" => !columnValue.Equals(comparisonValue),
                _ => throw new InvalidOperationException($"Invalid operator for string comparison: {operatorSymbol}"),
            };
        }

        // Method for DateTime comparison based on the operator
        private static bool CompareDateTime(string columnValue, string comparisonValue, string operatorSymbol)
        {
            if (DateTime.TryParseExact(columnValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime columnDateTime) &&
                DateTime.TryParseExact(comparisonValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime comparisonDateTime))
            {
                return operatorSymbol switch
                {
                    "=" => columnDateTime == comparisonDateTime,
                    "!=" => columnDateTime != comparisonDateTime,
                    ">" => columnDateTime > comparisonDateTime,
                    "<" => columnDateTime < comparisonDateTime,
                    ">=" => columnDateTime >= comparisonDateTime,
                    "<=" => columnDateTime <= comparisonDateTime,
                    _ => throw new InvalidOperationException($"Invalid operator for DateTime comparison: {operatorSymbol}"),
                };
            }
            return false; // Return false if parsing fails
        }

    }
}