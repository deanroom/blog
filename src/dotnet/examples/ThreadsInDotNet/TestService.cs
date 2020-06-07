using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsInDotNet
{
    public class TestService
    {
        public async Task<int> ProcessRequestAsync()
        {
            Common.WriteThreadInfo("ServiceRequest");
            return await Task.Run( async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                Common.WriteThreadInfo("ServiceResponse");
                return 1;
            }).ConfigureAwait(false);
        }
        public int ProcessRequest()
        {
            Common.WriteThreadInfo("ServiceRequest");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Common.WriteThreadInfo("ServiceResponse");
            return 1;

        }
    }
}