using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StoreSystem.CatalogOperations
{
    public class DropTable
    {
        public static bool isItEmpty(string tableName, string dbName)
        {
            string filePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table";
            FileInfo tableInfo = new FileInfo(filePath);

            if (tableInfo.Exists && tableInfo.Length == 0) return true;
            else return false;
        }

        public static void delTable (string tableName, string dbName)
        {
            string filePath = $@"C:\TinySQLDb\Databases\{dbName}\{tableName}.table";
            string tablesPath = @"C:\TinySQLDb\SystemCatalog\SystemTables.dat";
            string tempTablesPath = @"C:\TinySQLDb\SystemCatalog\TempSystemTables.dat";
            File.Delete(filePath);

            using (FileStream originalStream = File.Open(tablesPath, FileMode.Open, FileAccess.Read))
            using (FileStream temporalStream = File.Open(tempTablesPath, FileMode.Create, FileAccess.Write))
            using (BinaryReader originalReader = new BinaryReader(originalStream))
            using (BinaryWriter temporalWriter = new BinaryWriter(temporalStream))
            {
                while (originalStream.Position != originalStream.Length)
                {
                    string current = originalReader.ReadString();

                    if (!current.StartsWith($"{dbName},{tableName}"))
                    {
                        temporalWriter.Write(current);
                    }
                }

            }

            File.Delete(tablesPath);
            File.Move(tempTablesPath, tablesPath);
        }

        public static void delCols(string tableName, string dbName)
        {
            string colsPath = @"C:\TinySQLDb\SystemCatalog\SystemColumns.dat";
            string tempColsPath = @"C:\TinySQLDb\SystemCatalog\TempSystemColumns.dat";

            using (FileStream originalStream = File.Open(colsPath, FileMode.Open, FileAccess.Read))
            using (FileStream temporalStream = File.Open(tempColsPath, FileMode.Create, FileAccess.Write))
            using (BinaryReader originalReader = new BinaryReader(originalStream))
            using (BinaryWriter temporalWriter = new BinaryWriter(temporalStream))
            {
                while (originalStream.Position != originalStream.Length)
                {
                    string current = originalReader.ReadString();

                    if (!current.StartsWith($"{dbName},{tableName}"))
                    {
                        temporalWriter.Write(current);
                    }
                }
            }
            File.Delete(colsPath);
            File.Move(tempColsPath, colsPath);
        }
    }
}
