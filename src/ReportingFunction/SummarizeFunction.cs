using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace ReportingFunction
{
    public class SummarizeFunction
    {
        private readonly ILogger _logger;
        private readonly IKernel _sk;

        public SummarizeFunction(ILoggerFactory loggerFactory, IKernel sk)
        {
            _logger = loggerFactory.CreateLogger<SummarizeFunction>();
            _sk = sk;
        }

        [Function(nameof(SummarizeFunction))]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(
                                        AuthorizationLevel.Function
                                        , "post"
                                        , Route = "summarize")] HttpRequestData req)
        {
            // read in the request body
            var requestBody = req.ReadAsString();
            var response = req.CreateResponse(HttpStatusCode.OK);

            var summarizeFunction = _sk.Func("SummarizeSkill", "Error");


            var summary = await summarizeFunction.InvokeAsync(requestBody);

            if (summary.ErrorOccurred)
            {
                // return the error occurance from summary
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync(((Microsoft.SemanticKernel.AI.AIException)summary.LastException)?.Detail);
            }
            else
            {
                // create a response with summary as the body
                await response.WriteStringAsync(summary.Result);
            }

            return response;
        }
    }
}
