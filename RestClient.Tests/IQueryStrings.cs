using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    interface IQueryStrings
    {
        [Post]
        [Route(TestData.Routes.Empty)]
        Response PrimitiveWithDefaultName([InQueryString]int number);

        [Post]
        [Route(TestData.Routes.Empty)]
        Response PrimitiveWithSpecifiedName([InQueryString(Name = "SpecifiedName")]int number);
    }
}
