using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LiveHealthy
{
    public static class RetrieveUserPassword
    {
        [FunctionName("RetrieveUserPassword")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Users",
                collectionName: "users",
                ConnectionStringSetting = "cosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")] IEnumerable<UserProfile> userProfiles,
            ILogger log)
        {

            log.LogInformation("Retrieve password was called.");

            string pFirstName = req.Query["firstName"];
            string pLastName = req.Query["lastName"];
            string pEmail = req.Query["email"];

            UserHttpResponse response = new UserHttpResponse();
            response.message = "Fail: Unknown";
            response.error = (int)UserHttpErrorCodes.UNKNOWN_ERROR;

            if (string.IsNullOrEmpty(pFirstName))
            {
                response.error = (int)UserHttpErrorCodes.MISSING_FIRSTNAME;
                response.message = "Error: First name is missing.";
            }
            else if (string.IsNullOrEmpty(pLastName))
            {
                response.error = (int)UserHttpErrorCodes.MISSING_LASTNAME;
                response.message = "Error: Last name is missing.";
            }
            else if (string.IsNullOrEmpty(pEmail))
            {
                response.error = (int)UserHttpErrorCodes.MISSING_EMAIL;
                response.message = "Error: User email is missing.";
            }
            else
            {
                foreach (UserProfile user in userProfiles)
                {
                    if ( pEmail.ToLower() == user.pEmail.ToLower() && pFirstName.ToLower() == user.pFirstName.ToLower() && pLastName.ToLower() == user.pLastName.ToLower() )
                    {
                        response.error = (int) UserHttpErrorCodes.SUCCESS_STATUS;
                        response.message = "Success! we found your profile in our database. Please set a new password.";
                        response.data = user;
                        break;
                    }
                }
            }

            if (response.error == (int) UserHttpErrorCodes.UNKNOWN_ERROR){
                response.error = (int) UserHttpErrorCodes.PROFILE_DOES_NOT_EXIST;
                response.message = "Fail! your profile does not exists in our databse. Please make sure you enter correct information.";
            }

            return new OkObjectResult(response);
        }
    }
}
