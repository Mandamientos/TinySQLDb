using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem.CatalogOperations
{
    public class UpdateTable
    {
        private const string tabDBPath = @"C:\TinySQLDb\";

        public static void execute(string dbName, List<string> update, int index)
        {
            string path = tabDBPath + $@"Databases\{dbName}\{update[0]}.table";
            byte[] fileBytes = File.ReadAllBytes(path);
            string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);
            var lines = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                // Dividir la línea en columnas
                var columns = lines[i].Split(',');

                // Comprobar si el índice es válido
                if (index < columns.Length)
                {
                    // Modificar el valor en la columna específica
                    columns[index] = update[2];

                    // Reconstruir la línea con el nuevo valor
                    lines[i] = string.Join(",", columns);
                }
                else
                {
                    Console.WriteLine($"Índice de columna {index} no es válido para la línea {i}.");
                }
            }
            string modifiedContent = string.Join("\n", lines);
            byte[] modifiedBytes = System.Text.Encoding.UTF8.GetBytes(modifiedContent);
            Console.WriteLine(modifiedContent);
            Console.WriteLine(modifiedBytes);

            // Escribir el archivo binario con los cambios
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(modifiedBytes);
            }

            Console.WriteLine("El valor se ha modificado con éxito.");

        }

        public static void execute(string dbName, List<string> update, int index, int whereColumnIndex)
        {
            string path = tabDBPath + $@"Databases\{dbName}\{update[0]}.table";
            byte[] fileBytes = File.ReadAllBytes(path);
            string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);
            var lines = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                // Dividir la línea en columnas
                var columns = lines[i].Split(',');

                // Comprobar si el índice es válido
                if (index < columns.Length && whereColumnIndex < columns.Length)
                {
                    // Comprobar si el valor en la columna de condición coincide
                    if (columns[whereColumnIndex].Trim() == update[4].Trim())
                    {
                        // Modificar el valor en la columna específica
                        columns[index] = update[2];

                        // Reconstruir la línea con el nuevo valor
                        lines[i] = string.Join(",", columns);
                    }
                }
                else
                {
                    Console.WriteLine($"Índice de columna {index} o índice de condición {whereColumnIndex} no es válido para la línea {i}.");
                }
            }

            string modifiedContent = string.Join("\n", lines);
            Console.WriteLine(modifiedContent);
            byte[] modifiedBytes = System.Text.Encoding.UTF8.GetBytes(modifiedContent);
            Console.WriteLine(modifiedBytes);
            // Escribir el archivo binario con los cambios
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(modifiedBytes);
            }

            Console.WriteLine("Los valores se han modificado con éxito.");
        }

    }
}