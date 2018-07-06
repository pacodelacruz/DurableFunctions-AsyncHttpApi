using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class GetRawStatus
    {
        /// <summary>
        /// Returns the raw response from CreateCheckStatusResponse of the Durable Functions extension
        /// I don't recomend to publish this directly to third party clients, as they will be able to not only 
        /// get the status of a running instance, but also get inputs and outputs of each action, 
        /// send events to the Orchestration, and terminate the instance.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="orchestrationClient"></param>
        /// <param name="instanceId"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetRawStatus")]
        public static async Task<HttpResponseMessage> Run(
                                                    [HttpTrigger(AuthorizationLevel.Anonymous, methods: "get", Route = "rawcheckstatusresponse/{instanceId}")] HttpRequestMessage req,
                                                    [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
                                                    string instanceId,
                                                    ILogger logger)
        {
            return orchestrationClient.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
