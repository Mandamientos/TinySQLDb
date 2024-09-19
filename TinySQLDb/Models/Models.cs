using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Models
{
    public enum operationStatus
    {
        Success,
        Error,
        Warning
    }

    public class Request
    {
        public required string RequestBody { get; set; }
    }

    public class Response
    {

    }
}
