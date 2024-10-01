using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class GetTableData
    {
        public static List<List<String>> getData(string dbName, string tableName)
        {
            string dbTablePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table";

            List<List<String>> dataMatrix = new List<List<String>>();

            using (BinaryReader reader = new BinaryReader(File.Open(dbTablePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentString = reader.ReadString();
                    if(!string.IsNullOrEmpty(currentString))
                    {
                        dataMatrix.Add(new List<String>(currentString.Split(',')));
                    }
                }

            }

            return dataMatrix;
        }
    }
}
