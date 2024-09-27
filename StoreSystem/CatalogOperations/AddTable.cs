using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class AddTable
    {
        private const string tabDBPath = @"C:\TinySQLDb\SystemCatalog\SystemTables.dat";

        public static void execute(string dbName, string tabName, string column, string type)
        {
            using (FileStream stream = new FileStream(tabDBPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    using (BinaryWriter writer = new(stream))
                    {
                        writer.Write("DATABASE,TABLE\n"); 
                    }
                }
            }

            using (FileStream stream = File.Open(tabDBPath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                string result = $"{dbName},{tabName}\n";
                writer.Write(result);
            }
        }

        public static bool checkExistence(string dbName, string tabName)
        {
            string result = $"{dbName},{tabName}";
            using (BinaryReader reader = new BinaryReader(File.Open(tabDBPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentTable = reader.ReadString();

                    if (currentTable.Contains(result, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
