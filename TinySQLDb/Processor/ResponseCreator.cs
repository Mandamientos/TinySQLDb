using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ApiInterface.Models;
using QueryProcessing;
using Entities;

namespace ApiInterface.Processor
{
    internal class ResponseCreator(Request request)
    {
        public Request Request { get; set; } = request;

        public Response responseObject()
        {
            var result = SQLProcessor.executeQuery(this.Request.RequestBody);
            var response = convertToResponse(result);
            return response;
        }

        private Response convertToResponse(OperationStatus result)
        {
            return new Response
            {
                Status = result,
                Request = this.Request,
                ResponseBody = string.Empty
            };
        }
    }
}
