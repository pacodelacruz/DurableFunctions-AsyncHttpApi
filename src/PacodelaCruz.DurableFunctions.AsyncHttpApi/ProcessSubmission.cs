using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;
using System.Threading.Tasks;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class ProcessSubmission
    {
        /// <summary>
        /// Durable Functions Orchestration
        /// Receives a Call-for-Speaker submissions and control the approval workflow. 
        /// Updates the Orchestration Instance Custom Status as it progresses. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
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