using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    interface IUrlVariables
    {
        [Post]
        [Route("/{value.TextContent}")]
        Response DereferencedComplexObject(Request value);

        [Post]
        [Route("/{number}")]
        Response DirectPrimitiveInclusion(int number);

        [Post]
        [Route("/{number}/{otherNumber}")]
        Response MultipleDirectPrimitiveInclusion(int number, int otherNumber);

        [Post]
        [Route("/{number}/{request.TextContent}")]
        Response DirectPrimitiveAndComplexDereference(int number, Request request);
    }
}
