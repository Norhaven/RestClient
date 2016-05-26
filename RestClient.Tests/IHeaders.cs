using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    interface IHeaders
    {
        [Post]
        [Route(TestData.Routes.Empty)]
        Response PrimitiveWithDefaultName([InHeader]int number);

        [Post]
        [Route(TestData.Routes.Empty)]
        Response PrimitiveWithSpecifiedName([InHeader(Name = "SpecifiedName")]int number);
    }
}
