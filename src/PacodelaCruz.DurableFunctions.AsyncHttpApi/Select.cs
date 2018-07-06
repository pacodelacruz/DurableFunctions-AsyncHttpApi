using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PacodelaCruz.DurableFunctions.AsyncHttpApi.Models;

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