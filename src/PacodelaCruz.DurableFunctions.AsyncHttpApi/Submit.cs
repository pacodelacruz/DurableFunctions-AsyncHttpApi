
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class Submit
    {
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

            string checkStatusLocacion = string.Format("{0}://{1}/api/status/{2}", req.Scheme, req.Host, instanceId);
            string message = $"Your submission has been received. To get the status, go to: {checkStatusLocacion}"; // To inform the client where to check the status

            ActionResult response = new AcceptedResult(checkStatusLocacion, message);
            req.HttpContext.Response.Headers.Add("retry-after", "20"); // To inform the client how long to wait before checking the status.  

            return response;
        }
    }
}
