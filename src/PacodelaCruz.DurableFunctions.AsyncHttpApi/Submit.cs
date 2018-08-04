
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;
using System.IO;
using System.Threading.Tasks;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class Submit
    {
        /// <summary>
        /// HTTP Triggered Function which implements the DurableOrchestrationClient. 
        /// Receives a Call-for-Speakers submission as a POST with a JSON payload as the body request
        /// Then starts the submission processing orchestration. 
        /// Returns the location to check for the status by using the location and retry-after Http headers. 
        /// I'm using Anonymous Aurhotisation Level for demonstration purposes. You must use a more secure approach. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="orchestrationClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("Submit")]
        public static async Task<IActionResult> Run(
                                        [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "submit")]
                                        HttpRequest req,
                                        [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
                                        ILogger logger)
        {
            logger.LogInformation("Submission received via Http");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var submission = JsonConvert.DeserializeObject<Presentation>(requestBody, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var instanceId = await orchestrationClient.StartNewAsync("ProcessSubmission", submission);
            logger.LogInformation("Submission process started", instanceId);

            string checkStatusLocacion = string.Format("{0}://{1}/api/status/{2}", req.Scheme, req.Host, instanceId); // To inform the client where to check the status
            string message = $"Your submission has been received. To get the status, go to: {checkStatusLocacion}"; 

            // Create an Http Response with Status Accepted (202) to let the client know that the request has been accepted but not yet processed. 
            ActionResult response = new AcceptedResult(checkStatusLocacion, message); // The GET status location is returned as an http header
            req.HttpContext.Response.Headers.Add("retry-after", "20"); // To inform the client how long to wait in seconds before checking the status

            return response;
        }
    }
}
