using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class AddTable
    {
        private const string tabDBPath = @"C:\TinySQLDb\";

        public static void execute(string dbName, string tabName)
        {
            string path = tabDBPath + @"SystemCatalog\SystemTables.dat";
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    using (BinaryWriter writer = new(stream))
                    {
                        writer.Write("DATABASE,TABLE\n"); 
                    }
                }
            }

            using (FileStream stream = File.Open(path, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                string result = $"{dbName},{tabName}\n";
                writer.Write(result);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(dbName);
            Console.WriteLine();
            Console.WriteLine();
            path = tabDBPath + $@"\Databases\{dbName}\{tabName}.table";
            try
            {
                using (FileStream stream = File.Open(path, FileMode.CreateNew, FileAccess.Write))
                {
                    // Empty file is created
                }
            }
            catch (IOException)
            {
                Console.WriteLine("File already exists.");
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
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
