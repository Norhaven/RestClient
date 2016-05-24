using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    interface IPostBody
    {
        [Post]
        [Route("/novariables")]
        Response Run([InBody]Request value);
    }
}
