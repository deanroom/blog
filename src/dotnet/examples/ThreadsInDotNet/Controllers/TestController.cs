using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ThreadsInDotNet.Controllers
{
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<int>> HttpRequestAsync()
        {
            Common.WriteThreadInfo("ControllerRequest");
            var result = await new TestService().ProcessRequestAsync();
            Common.WriteThreadInfo("ControllerResponse");
            return new OkObjectResult(result);
        }
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public ActionResult<int> HttpRequest2()
        {
            Common.WriteThreadInfo("ControllerRequest");
            var result = new TestService().ProcessRequest();
            Common.WriteThreadInfo("ControllerResponse");
            return new OkObjectResult(result);
        }
    }
}