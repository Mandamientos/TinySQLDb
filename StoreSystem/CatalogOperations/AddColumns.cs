using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class AddColumns
    {
        private const string colDBPath = @"C:\TinySQLDb\SystemCatalog\SystemColumns.dat";

        public static void execute(string dbName, string tabName,Dictionary<string, (string DataType, bool IsNullable, List<string> Constraints)> createcolumns)
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
                foreach (var col in createcolumns)
                {
                    string result = $"{dbName},{tabName},{col.Key},{col.Value.DataType},{col.Value.IsNullable},{string.Join("," , col.Value.Constraints)}\n";
                    Console.WriteLine($"{col.Key}");
                    writer.Write(result);
                }
            }
        }
    }
}
