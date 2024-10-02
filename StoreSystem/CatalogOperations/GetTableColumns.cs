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
        public static List<List<string>> GetTableNameColumns(string dbName, string tableName)
        {
            List<List<string>> columns = new List<List<string>>();

            using(BinaryReader reader = new BinaryReader(File.Open(colDBPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentString = reader.ReadString();
                    if(currentString.StartsWith($"{dbName},{tableName}"))
                    {
                        string[] tempArray = currentString.Split(',');
                        columns.Add(new List<String> { tempArray[2], tempArray[3] });
                    }
                }
            }

            return columns;
        }
    }
}
