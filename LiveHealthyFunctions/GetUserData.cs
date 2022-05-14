using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LiveHealthy
{
    public static class GetUserData
    {
        [FunctionName("GetUserData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "LiveHealthyDB",
                collectionName: "LiveHealthyDbUserData",
                ConnectionStringSetting = "cosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")] IEnumerable<UserData> userDataArray,
            ILogger log)
        {
            log.LogInformation("Get user data endpoints are called.");

            string pEmail = req.Query["email"];
            string pPassword = req.Query["password"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            pEmail = pEmail ?? data?.email;
            pPassword = pPassword ?? data?.password;


            DataHTttpResponse response = new DataHTttpResponse();
            List<UserData> samples = new List<UserData>();

            if (string.IsNullOrEmpty(pEmail))
            {
                response.error = (int)UserHttpErrorCodes.MISSING_EMAIL;
                response.message = "Error: User email is missing.";
            }
            else if (string.IsNullOrEmpty(pPassword))
            {
                response.error = (int)UserHttpErrorCodes.MISSING_PASSOWRD;
                response.message = "Error: User password is missing.";
            }
            else
            {
                foreach (UserData row in userDataArray)
                {
                    if (pPassword == row.pPassword && pEmail == row.pEmail)
                    {
                        samples.Add(row);
                    }
                }

                if (samples.Count > 0)
                {
                    response.error = (int)UserHttpErrorCodes.SUCCESS_STATUS;
                    response.message = "Success! your data is retrieved.";
                    response.data = samples;
                }
                else
                {
                    response.message = "Failure! unable to find your email address and/or password in the user data collection.";
                    response.error = (int)UserHttpErrorCodes.EMAIL_AND_OR_PASSWORD_NOT_FOUND;
                }

            }


            return new OkObjectResult(response);
        }
    }
}
