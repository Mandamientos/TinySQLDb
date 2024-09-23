using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ApiInterface.Models
{

    public class Request
    {
        public required string RequestBody { get; set; }
    }

    public class Response
    {
        public required Request Request { get; set; }
        public required OperationStatus Status { get; set; }
        public required string ResponseBody { get; set; }
    }
}
