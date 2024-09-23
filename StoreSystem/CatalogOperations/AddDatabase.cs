using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StoreSystem.SystemCatalog;

namespace StoreSystem.CatalogOperations
{
    public class AddDatabase
    {
        private const string sysDBPath = @"C:\TinySQLDb\SystemCatalog\SystemDatabases.dat";
        public static void execute(string dbName)
        {
            using (FileStream stream = File.Open(sysDBPath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                writer.Write(dbName);
            }
        }

        public static bool checkExistence(string dbName)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(sysDBPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string currentDbName = reader.ReadString();

                    if(currentDbName.Equals(dbName.Trim(), StringComparison.OrdinalIgnoreCase)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
