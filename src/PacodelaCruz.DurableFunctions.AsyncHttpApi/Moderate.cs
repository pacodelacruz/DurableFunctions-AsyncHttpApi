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
    /// <summary>
    /// Dummy Durable Activity Function to simulate a manual moderation step.
    /// </summary>
    public static class Moderate
    {
        [FunctionName("Moderate")]
        public static bool RunActivity(
                                [ActivityTrigger] Presentation presentationDetails, 
                                ILogger logger)
        {
            bool approved = true;

            Random random = new Random();
            int randomInt = random.Next(6);
            // low chances of being rejected at moderation
            if (randomInt == 0)
                approved = false;
            else
                approved = true;

            Thread.Sleep(20000);

            return approved;
        }
    }
}