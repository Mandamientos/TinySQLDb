using StoreSystem.CatalogOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Text.RegularExpressions;
using QueryProcessing.SQLParser;

namespace QueryProcessing.Operations
{
    public class Create_Index
    {

        private static string indexName;
        private static string tableName;
        private static string columnName;
        private static string indexType;
        private static string dbName;

        public static (OperationStatus, string) execute(string sql)
        {
            setValues(sql);
            dbName = SQLProcessor.selectedDB;
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
                    crearIndex();
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

        // Method to load indexes and create trees
        public static void cargarIndexes()
        {
            string path = @"C:\TinySQLDb\SystemCatalog\SystemIndexes.dat"; // Adjust the path as necessary
            if (File.Exists(path))
            {
                // Use BinaryReader to read from the binary file
                using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    try
                    {
                        // Read until the end of the file
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            // Read each line from the binary file
                            string line = reader.ReadString();

                            if (!string.IsNullOrWhiteSpace(line))
                            {

                                // Split the line by commas and insert each value into the new tree
                                List<string> values = new List<string>(line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                                crearIndex(values[0], values[1], values[3]);
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        // Handle end of file gracefully if needed
                    }
                }
            }
        }

        public static void crearIndex()
        {
            ArbolBinario newTree = new ArbolBinario();
            List<List<string>> cols = GetTableColumns.GetTableNameColumns(dbName, tableName);
            int i = 0;

            // Find the index for the column that matches 'columnName'
            foreach (List<string> col in cols)
            {
                if (col[0] == columnName) { break; }
                i++;
            }

            string filePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table"; // Replace with your binary file path

            // Open the binary file and read its content
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the binary data into a byte array
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);

                // Convert the byte array to a string (use the correct encoding if known)
                string content = Encoding.UTF8.GetString(bytes);

                // Split the string by lines
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // Process each line and split by commas
                foreach (string line in lines)
                {
                    // Split each line by commas and store as a list of strings
                    List<string> values = new List<string>(line.Split(new[] { ',' }, StringSplitOptions.None));

                    // Ensure the line has enough values to get the key at index i
                    if (values.Count > i)
                    {
                        // Use values[i] as the key and the entire list of values as the data
                        string key = values[i];
                        List<string> data = new List<string>(values);  // Use the entire values list as data

                        // Insert into the binary tree
                        newTree.Insertar(key, data);
                        GlobalTreeStorage.Trees.Add(newTree);
                    }
                }
            }
        }


        public static void crearIndex(string dbName, string tableName, string columnName)
        {
            ArbolBinario newTree = new ArbolBinario();
            List<List<string>> cols = GetTableColumns.GetTableNameColumns(dbName, tableName);
            int i = 0;

            // Find the index for the column that matches 'columnName'
            foreach (List<string> col in cols)
            {
                if (col[0] == columnName) { break; }
                i++;
            }

            string filePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table"; // Replace with your binary file path

            // Open the binary file and read its content
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the binary data into a byte array
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);

                // Convert the byte array to a string (use the correct encoding if known)
                string content = Encoding.UTF8.GetString(bytes);

                // Split the string by lines
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // Process each line and split by commas
                foreach (string line in lines)
                {
                    // Split each line by commas and store as a list of strings
                    List<string> values = new List<string>(line.Split(new[] { ',' }, StringSplitOptions.None));

                    // Ensure the line has enough values to get the key at index i
                    if (values.Count > i)
                    {
                        // Use values[i] as the key and the entire list of values as the data
                        string key = values[i];
                        List<string> data = new List<string>(values);  // Use the entire values list as data

                        // Insert into the binary tree
                        newTree.Insertar(key, data);
                        GlobalTreeStorage.Trees.Add(newTree);
                    }
                }
            }
        }


    }
}
