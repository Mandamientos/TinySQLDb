using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using QueryProcessing.Operations;
using System.Text.RegularExpressions;
using QueryProcessing.Models;
using System.Diagnostics;

namespace QueryProcessing.SQLParser
{
    public class Parser
    {
        private static string TableName;
        private static Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)> CreateColumns;
        private static List<string> inserts;

        public static (OperationStatus, string) sentenceParser(string sentence)
        {
            if (sentence.StartsWith("CREATE DATABASE"))
            {
                return Create_Database.execute(sentence);
            }

            else if (sentence.StartsWith("SET DATABASE"))
            {
                return Set_Database.execute(sentence);
            }
            else if (createTableParse(sentence))
            {
                return Create_Table.execute(TableName, CreateColumns);
            }
            else if (sentence.StartsWith("DROP TABLE"))
            {
                return Drop_Table.execute(sentence.Substring("DROP TABLE ".Length).Trim());
            }
            else if (insertInto(sentence))
            {
                Console.WriteLine("Si cumplio el insert into");
                return Insert_Into.execute(TableName, inserts); ;
            } 
            else if (parseSelectTable(sentence))
            {
                return Select_Table.execute(createSelectModel(sentence));
            }
            else if(updateParse(sentence) != null)
            {
                return Update_Table.execute(updateParse(sentence));
            }


            return (OperationStatus.Error, "There might be an error in your syntax; please check it.");
        }

        private static bool createTableParse(string sql) 
        {
            CreateColumns = new Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)>();
            var pattern = @"CREATE TABLE\s+(?<tableName>\w+)\s*\((?<columns>.+?)\)\s*;";
            sql = sql + ";";
            if (ParseCreateTableStatement(sql, pattern)) 
            {
                return true;
            }
            return false;
        }

        private static List<string> updateParse(string sql)
        {
            Console.WriteLine("entro al update");
            List<string> extract = null;
            var pattern = @"UPDATE\s+(?<tableName>\w+)\s+SET\s+(?<column>\w+)\s*=\s*(?<value>'?\w+'?)\s*(WHERE\s+(?<columnName>\w+)\s*=\s*(?<value2>'?\w+'?))?";
            sql = sql.Replace("\\u0027", "'");
            var match = Regex.Match(sql, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // Extracting the mandatory fields
                string tableName = match.Groups["tableName"].Value;
                string column = match.Groups["column"].Value;
                string  value = match.Groups["value"].Value.Replace("'", "");
                extract = new List<string> { tableName, column, value};

                // Checking if WHERE clause is present
                if (match.Groups["columnName"].Success && match.Groups["value2"].Success)
                {
                    string columnName = match.Groups["columnName"].Value;
                    string value2 = match.Groups["value2"].Value.Replace("'", "");
                    extract.AddRange(new[] { columnName, value2 });
                }
            }
            return extract;
        }



        private static bool insertInto(string sql) 
        {
            Console.WriteLine("entro al inser into a validar");
            inserts= new List<string>();
            var pattern = @"Insert Into\s+(?<tableName>\w+)\s*\((?<inserts>.+?)\)";
            if (ParseInsertStatement(sql, pattern))
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

        private static bool ParseInsertStatement(string sql, string pattern)
        {
            // Match the INSERT INTO statement using the provided pattern
            var match = Regex.Match(sql, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success)
            {
                return false;
            }

            // Extract the table name
            TableName = match.Groups["tableName"].Value.Trim();

            // Extract the values part of the statement
            var valuesPart = match.Groups["inserts"].Value.Trim();
            inserts = ParseValues(valuesPart);


            return true;
        }

        private static List<string> ParseValues(string valuesPart)
        {
            var values = new List<string>();
            var currentValue = "";
            bool insideString = false;

            foreach (char c in valuesPart)
            {
                // Handle string literals that may contain commas
                if (c == '"')
                {
                    insideString = !insideString;
                }

                // If we encounter a comma and we're not inside a string, treat it as a separator
                if (c == ',' && !insideString)
                {
                    values.Add(currentValue.Trim());
                    currentValue = "";
                }
                else
                {
                    currentValue += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentValue))
            {
                values.Add(currentValue.Trim());
            }
            return values;
        }

        public static bool parseSelectTable(string sentence)
        {
            string patternFullQuery = @"^SELECT\s+(\*|[\w\s]+(,[\w\s]+)*)\s+FROM\s+(\w+)" + 
                @"(\s+WHERE\s+(\w+)\s*(==|>|<|NOT|LIKE)\s*('[^']*'|\d+|\w+))?" + 
                @"(\s+ORDER BY\s+(\w+)\s*(ASC|DESC))?$";
            Match patternMatch = Regex.Match(sentence, patternFullQuery, RegexOptions.IgnoreCase);

            if (!patternMatch.Success)
            {
                return false;
            }

            return true;
        }

        public static SelectModel createSelectModel(string sentence)
        {
            var sql = new SelectModel();

            string patternFullQuery = @"^SELECT\s+(\*|[\w\s]+(,[\w\s]+)*)\s+FROM\s+(\w+)" + 
                @"(\s+WHERE\s+(\w+)\s*(==|>|<|NOT|LIKE)\s*('[^']*'|\d+|\w+))?" + 
                @"(\s+ORDER BY\s+(\w+)\s*(ASC|DESC))?$";

            Match matchPattern = Regex.Match(sentence, patternFullQuery, RegexOptions.IgnoreCase);

            string cols = matchPattern.Groups[1].Value.Trim();
            if (cols == "*") sql.columns.Add("*");
            else
            {
                string[] colsList;
                colsList = cols.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string col in colsList) {
                    if (string.IsNullOrEmpty(col)) return null;
                    sql.columns.Add(col.Trim());
                }
            }

            string tableName = matchPattern.Groups[3].Value.Trim();
            sql.tableName = tableName;

            if (matchPattern.Groups[4].Success)
            {
                string whereColumn = matchPattern.Groups[5].Value.Trim();
                string whereOperator = matchPattern.Groups[6].Value.Trim();
                string whereValue = matchPattern.Groups[7].Value.Trim();

                sql.whereColumn = whereColumn;
                sql.whereComparator = whereOperator;
                sql.whereValue = whereValue;
            }

            return sql;
        }

    }
}


