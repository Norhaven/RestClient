using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    interface IHttpVerbs
    {
        [Post]
        [Route(TestData.Routes.Empty)]
        Response Post([InBody]Request value);

        [Put]
        [Route(TestData.Routes.Empty)]
        Response Put([InBody]Request value);

        [Delete]
        [Route(TestData.Routes.Empty)]
        Response Delete([InBody]Request value);

        [Get]
        [Route(TestData.Routes.Empty)]
        Response Get([InBody]Request value);

        [Options]
        [Route(TestData.Routes.Empty)]
        Response Options([InBody]Request value);

        [Patch]
        [Route(TestData.Routes.Empty)]
        Response Patch([InBody]Request value);

        [Head]
        [Route(TestData.Routes.Empty)]
        Response Head([InBody]Request value);
    }
}
