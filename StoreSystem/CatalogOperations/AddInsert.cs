using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StoreSystem.CatalogOperations
{

    public class AddInsert
    {
        private const string tabDBPath = @"C:\TinySQLDb\";

        public static void execute(string dbName, string tabName, List<string> inserts)
        {
            string path = tabDBPath + $@"Databases\{dbName}\{tabName}.table";
            using (FileStream stream = File.Open(path, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                string result = $"{string.Join(",", inserts)}\n";
                Console.WriteLine(result);
                writer.Write(result);
            }

        }

        public static bool checkExistence(string dbName, string tabName)
        {
            string path = tabDBPath + @"SystemCatalog\SystemTables.dat";
            string result = $"{dbName},{tabName}";
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentTable = reader.ReadString();

                    if (currentTable.Contains(result, StringComparison.OrdinalIgnoreCase))
                    {
                        return checkTableExistance(dbName, tabName);
                    }
                }
            }
            return false;
        }

        private static bool checkTableExistance(string dbname, string tabname)
        {
            string path = tabDBPath + $@"Databases\{dbname}\{tabname}.table";
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}