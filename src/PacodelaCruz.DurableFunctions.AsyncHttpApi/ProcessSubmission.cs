using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class ProcessSubmission
    {
        [FunctionName("ProcessSubmission")]
        public static async Task<bool> RunOrchestrator(
                                    [OrchestrationTrigger] DurableOrchestrationContext context,
                                    ILogger logger)
        {
            Presentation presentation = context.GetInput<Presentation>();
            presentation.Id = context.InstanceId;
            string stage;
            string status;
            bool isTrackingEvent = true;

            bool approved;

            stage = "Moderation";
            context.SetCustomStatus(stage);
            approved = await context.CallActivityAsync<bool>("Moderate", presentation);

            if (approved)
            {
                stage = "Shortlisting";
                context.SetCustomStatus("Shortlisting");
                approved = await context.CallActivityAsync<bool>("Shortlist", presentation);

                if (approved)
                {
                    stage = "Selection";
                    context.SetCustomStatus(stage);
                    approved = await context.CallActivityAsync<bool>("Select", presentation);
                }
            }
            if (approved)
                status = "Approved";
            else
                status = "Rejected";

            context.SetCustomStatus(status);
            logger.LogInformation("Submission has been {status} at stage {stage}. {presenter}, {title}, {track}, {speakerCountry}, {isTrackingEvent}", status, stage, presentation.Speaker.Email, presentation.Title, presentation.Track, presentation.Speaker.Country, isTrackingEvent);

            return approved;
        }
    }
}