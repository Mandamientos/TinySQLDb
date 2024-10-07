using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class AddIndex
    {
        private const string tabDBPath = @"C:\TinySQLDb\SystemCatalog\";

        public static void execute(string dbName, string tabName, string indexName, string colName, string type)
        {
            string path = tabDBPath + $@"SystemIndexes.dat";
            using (FileStream stream = File.Open(path, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                string result = $"{dbName},{tabName},{indexName},{colName},{type}\n";
                writer.Write(result);
            }

        }

        public static bool checkExistance(string dbName, string tabName, string col, string indexName, string type)
        {
            string path = tabDBPath + @"SystemIndexes.dat";
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentCol = reader.ReadString();

                    // Split the current line into its components
                    string[] components = currentCol.Split(',');

                    // First condition: {dbName},{anytable},{indexName}
                    bool condition1 = components.Length >= 3 &&
                                      components[0].Equals(dbName, StringComparison.OrdinalIgnoreCase) &&
                                      components[2].Equals(indexName, StringComparison.OrdinalIgnoreCase);

                    // Second condition: {dbName},{tableName},{anyindexName},{col},{type}
                    bool condition2 = components.Length >= 5 &&
                                      components[0].Equals(dbName, StringComparison.OrdinalIgnoreCase) &&
                                      components[1].Equals(tabName, StringComparison.OrdinalIgnoreCase) &&
                                      components[3].Equals(col, StringComparison.OrdinalIgnoreCase) &&
                                      components[4].Equals(type, StringComparison.OrdinalIgnoreCase);

                    if (condition1 || condition2)
                        return true;
                }
            }
            return false;
        }



        public static bool canBeIndex(string dbName, string tabName, string col)
        {
            string path = tabDBPath + @"SystemColumns.dat";
            string result = $"{dbName},{tabName},{col}";
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentCol = reader.ReadString();

                    if (
                        currentCol.Contains(result, StringComparison.OrdinalIgnoreCase) &&
                        (
                        currentCol.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                        currentCol.Contains("PRIMARYKEY", StringComparison.OrdinalIgnoreCase)
                        )
                        )
                        return true;     
                    }
                }
            return false;
        }

        
    }
}
