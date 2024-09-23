namespace StoreSystem.SystemCatalog
{
    public class SystemCatalog
    {
        private string systemCatalogPath = @"C:\TinySQLDb\SystemCatalog";
        public SystemCatalog()
        {
            this.initializeSystemCatalog();
        }

        private void initializeSystemCatalog()
        {
            if (!Directory.Exists(systemCatalogPath))
            {
                Directory.CreateDirectory(systemCatalogPath);
            }

            string[] filesToCreate = { "SystemDatabases.dat", "SystemTables.dat", "SystemColumns.dat", "SystemIndexes.dat" };

            foreach (string file in filesToCreate)
            {
                string path = Path.Combine(systemCatalogPath, file);
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
            }
        }
    }
}