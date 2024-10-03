using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessing.Models
{
    public class SelectModel
    {
        public List<string> columns { get; set; }
        public string tableName { get; set; }
        public string whereColumn { get; set; }
        public string whereComparator { get; set; }
        public object whereValue { get; set; }
        public string OrderDirection { get; set; }
        public string OrderColumn { get; set; }

        public SelectModel()
        {
            columns = new List<string>();
        }
    }
}
