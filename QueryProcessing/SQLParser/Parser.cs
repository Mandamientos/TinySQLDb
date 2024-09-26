using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using QueryProcessing.Operations;
using System.Text.RegularExpressions;

namespace QueryProcessing.SQLParser
{
    public class Parser
    {
        private static string TableName;
        private static Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)> CreateColumns;

        public static OperationStatus sentenceParser(string sentence)
        {
            if (sentence.StartsWith("CREATE DATABASE"))
            {
                return Create_Database.execute(sentence);
            }

            if (sentence.StartsWith("SET DATABASE"))
            {
                return Set_Database.execute(sentence);
            }
            if (createTableParse(sentence)) 
            {
                return OperationStatus.Success;
            }

            return OperationStatus.Error;
        }

        private static bool createTableParse(string sql) 
        {
            CreateColumns = new Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)>();
            var pattern = @"CREATE TABLE\s+(?<tableName>\w+)\s*\((?<columns>.+?)\)\s*";
            if (ParseCreateTableStatement(sql, pattern)) 
            {
                return true;
            }
            return false;
        }

        private static bool ParseCreateTableStatement(string sql, string pattern)
        {
            var match = Regex.Match(sql, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success)
            {
                return false;
            }
            TableName = match.Groups["tableName"].Value.Trim();
            var columnsPart = match.Groups["columns"].Value.Trim();
            var columnDefinitions = ParseColumns(columnsPart);
            foreach (var columnDef in columnDefinitions)
            {
                ParseColumnDefinition(columnDef.Trim());
            }
            return true;
        }

        private static List<string> ParseColumns(string columnsPart)
        {
            var columns = new List<string>();
            int parenthesisDepth = 0;
            var currentColumn = "";
            foreach (char c in columnsPart)
            {
                if (c == '(')
                {
                    parenthesisDepth++;
                }
                else if (c == ')')
                {
                    parenthesisDepth--;
                }

                if (c == ',' && parenthesisDepth == 0)
                {
                    columns.Add(currentColumn.Trim());
                    currentColumn = "";
                }
                else
                {
                    currentColumn += c;
                }
            }
            if (!string.IsNullOrWhiteSpace(currentColumn))
            {
                columns.Add(currentColumn.Trim());
            }
            return columns;
        }

        private static void ParseColumnDefinition(string columnDef)
        {
            var regex = new Regex(@"(?<name>\w+)\s+(?<type>INTEGER|DOUBLE|TIMESTAMP|VARCHAR\(\d+\)|DATETIME)\s*(?<nullable>NOT NULL|NULL)?(?<constraints>.*)", RegexOptions.IgnoreCase);
            var match = regex.Match(columnDef);
            if (match.Success)
            {
                string columnName = match.Groups["name"].Value.Trim();
                string dataType = match.Groups["type"].Value.Trim();
                bool isNullable = !string.Equals(match.Groups["nullable"].Value.Trim(), "NOT NULL", StringComparison.OrdinalIgnoreCase);
                var constraints = new List<string>();
                var constraintsString = match.Groups["constraints"].Value.Trim();
                if (!string.IsNullOrEmpty(constraintsString))
                {
                    constraints.AddRange(ExtractConstraints(constraintsString));
                }
                CreateColumns[columnName] = (dataType, isNullable, constraints);
            }
        }

        private static List<string> ExtractConstraints(string constraintsString)
        {
            var constraints = new List<string>();
            var splitConstraints = constraintsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string currentConstraint = "";
            foreach (var part in splitConstraints)
            {
                if (currentConstraint.Length > 0)
                {
                    currentConstraint += " ";
                }
                currentConstraint += part;

                if (IsCompleteConstraint(currentConstraint))
                {
                    constraints.Add(currentConstraint);
                    currentConstraint = "";
                }
            }

            if (!string.IsNullOrEmpty(currentConstraint))
            {
                constraints.Add(currentConstraint);
            }

            return constraints;
        }

        static bool IsCompleteConstraint(string constraint)
            {
                var knownConstraints = new List<string> { "PrimaryKey", "ForeignKey", "Unique", "Check", "INCREMENTAL" };

                return knownConstraints.Contains(constraint, StringComparer.OrdinalIgnoreCase);
            }
        }
}
