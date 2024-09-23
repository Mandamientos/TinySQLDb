using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using QueryProcessing.Operations;

namespace QueryProcessing.SQLParser
{
    public class Parser
    {
        public static OperationStatus sentenceParser(string sentence)
        {
            if (sentence.StartsWith("CREATE DATABASE"))
            {
                return Create_Database.execute(sentence);
            }

            if (sentence.StartsWith("SET DATABASE"))
            {
                return Set_Database.execute(sentence);
            }

            return OperationStatus.Error;
        } 
    }
}
