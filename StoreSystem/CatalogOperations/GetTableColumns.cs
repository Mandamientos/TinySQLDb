using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class GetTableColumns
    {
        private const string colDBPath = @"C:\TinySQLDb\SystemCatalog\SystemColumns.dat";
        public static List<string> GetTableNameColumns(string dbName, string tableName)
        {
            List<string> columns = new List<string>();

            using(BinaryReader reader = new BinaryReader(File.Open(colDBPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentString = reader.ReadString();
                    if(currentString.StartsWith($"{dbName},{tableName}"))
                    {
                        string[] tempArray = currentString.Split(',');
                        Console.WriteLine(tempArray[2]);
                        columns.Add(tempArray[2]);
                    }
                }
            }

            return columns;
        }
    }
}
