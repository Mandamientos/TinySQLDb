using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class AddColumns
    {
        private const string colDBPath = @"C:\TinySQLDb\SystemCatalog\SystemColumns.dat";

        public static void execute(string dbName, string tabName, string column, string type, bool nul, string constraint)
        {
            using (FileStream stream = new FileStream(colDBPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    using (BinaryWriter writer = new(stream))
                    {
                        writer.Write("DATABASE,TABLE,COLUMN,TYPE,NULLABILITY,CONSTRAINT\n");
                    }
                }
            }

            using (FileStream stream = File.Open(colDBPath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {

                string result = $"{dbName},{tabName},{column},{type},{nul},{constraint}\n";
                writer.Write(result);
            }
        }
    }
}
