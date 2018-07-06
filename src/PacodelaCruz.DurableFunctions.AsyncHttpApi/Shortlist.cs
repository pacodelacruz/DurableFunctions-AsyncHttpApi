using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;
using System;
using System.Threading;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    /// <summary>
    /// Dummy Durable Activity Function to simulate a manual shortlisting step.
    /// </summary>
    public static class Shortlist
    {

        [FunctionName("Shortlist")]
        public static bool RunActivity(
                                [ActivityTrigger] Presentation presentationDetails,
                                ILogger logger)
        {
            bool approved = true;

            Random random = new Random();
            int randomInt = random.Next(2);

            // 50% changes of getting shortlisted
            if (randomInt == 0)
                approved = false;
            else
                approved = true;

            Thread.Sleep(15000);

            return approved;
        }
    }
}