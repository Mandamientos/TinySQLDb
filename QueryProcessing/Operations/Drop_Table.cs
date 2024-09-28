﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreSystem.CatalogOperations;

namespace QueryProcessing.Operations
{
    internal class Drop_Table
    {
        public static OperationStatus execute(string tableName)
        {
            if(string.IsNullOrEmpty(tableName))
            {
                return OperationStatus.Error;
            } else if (AddTable.checkExistence(SQLProcessor.selectedDB, tableName) && DropTable.isItEmpty(tableName, SQLProcessor.selectedDB)) {
                
                DropTable.delTable(tableName, SQLProcessor.selectedDB);
                DropTable.delCols(tableName, SQLProcessor.selectedDB);

                return OperationStatus.Success;
            }

            return OperationStatus.Error;
        }
    }
}
