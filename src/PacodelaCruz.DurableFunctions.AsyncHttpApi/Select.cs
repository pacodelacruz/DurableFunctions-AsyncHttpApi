using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;
using System;
using System.Threading;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi
{
    public static class Select
    {

        /// <summary>
        /// Dummy Durable Activity Function to simulate a manual final selection step.
        /// </summary>
        [FunctionName("Select")]
        public static bool RunActivity(
                                [ActivityTrigger] Presentation presentationDetails,
                                ILogger logger)
        {
            bool approved = true;

            Random random = new Random();
            int randomInt = random.Next(6);

            // Low changes of getting approved at selection
            if (randomInt < 2)
                approved = false;
            else
                approved = true;

            Thread.Sleep(10000);

            return approved;
        }
    }
}