using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class GetStatus
    {
        /// <summary>
        /// Http Triggered Function which acts as a wrapper to get the status of a running Durable orchestration instance.
        /// It enriches the response based on the GetStatusAsync's retruned value
        /// I'm using Anonymous Aurhotisation Level for demonstration purposes. You should use a more secure approach. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="orchestrationClient"></param>
        /// <param name="instanceId"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetStatus")]
        public static async Task<IActionResult> Run(
                                                    [HttpTrigger(AuthorizationLevel.Anonymous, methods: "get", Route = "status/{instanceId}")] HttpRequest req,
                                                    [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
                                                    string instanceId,
                                                    ILogger logger)
        {
            var status = await orchestrationClient.GetStatusAsync(instanceId);
            if (status != null)
            {
                string customStatus = (string)status.CustomStatus;
                if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running || status.RuntimeStatus == OrchestrationRuntimeStatus.Pending)
                {
                    string checkStatusLocacion = string.Format("{0}://{1}/api/status/{2}", req.Scheme, req.Host, instanceId);
                    string message = $"Your submission is being processed. The current status is {customStatus}. To check the status later, go to: GET {checkStatusLocacion}"; // To inform the client where to check the status

                    ActionResult response = new AcceptedResult(checkStatusLocacion, message);
                    req.HttpContext.Response.Headers.Add("retry-after", "20"); // To inform the client how long to wait before checking the status. 
                    return response;
                }
                else if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    if (customStatus == "Approved")
                        return new OkObjectResult($"Congratulations, your presentation with id '{instanceId}' has been accepted!");
                    else
                        return new OkObjectResult($"We are sorry! Unfortunately your presentation with id '{instanceId}' has not been accepted.");
                }
            }
            return new OkObjectResult($"Whoops! Something went wrong. Please check if your submission Id is correct. Submission '{instanceId}' not found.");
        }
    }
}
