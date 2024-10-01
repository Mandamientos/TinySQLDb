using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryProcessing.SQLParser;
using Entities;

namespace QueryProcessing
{
    public class SQLProcessor()
    {
        public static string selectedDB;
        public static (OperationStatus, string) executeQuery(string sentence)
        {
            return Parser.sentenceParser(sentence);
        }
    }
}
