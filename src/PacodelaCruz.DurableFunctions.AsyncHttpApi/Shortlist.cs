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