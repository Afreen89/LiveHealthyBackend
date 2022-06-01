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
    public static class SignInFunction
    {
        [FunctionName("SignInFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "UserData",
                collectionName: "userprofile",
                ConnectionStringSetting = "cosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")] IEnumerable<UserProfile> userProfiles,
            ILogger log
        )
        {

            log.LogInformation("Sign In API is called");

            string pEmail = req.Query["email"];
            string pPassword = req.Query["password"];
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            pEmail = pEmail ?? data?.email;
            pPassword = pPassword ?? data?.password;
            
            UserHttpResponse response  = new UserHttpResponse();
            response.message = "Fail: Unknown";
            response.error =  (int) UserHttpErrorCodes.UNKNOWN_ERROR;
            
            bool foundEmailButNotPassword = false;


            if ( string.IsNullOrEmpty(pEmail)){
                response.error =  (int) UserHttpErrorCodes.MISSING_EMAIL;
                response.message = "Error: User email is missing.";
            } else if ( string.IsNullOrEmpty( pPassword ) ){
                response.error = (int)  UserHttpErrorCodes.MISSING_PASSOWRD;
                response.message = "Error: User password is missing.";
            } else {
                foreach( UserProfile user in userProfiles ){
                    if ( pPassword == user.pPassword && pEmail == user.pEmail ){
                        response.error =  (int) UserHttpErrorCodes.SUCCESS_STATUS;
                        response.message = "Success! you are signed in now.";
                        response.data = user;
                        break;
                    } else if ( pPassword != user.pPassword && pEmail == user.pEmail ){
                        foundEmailButNotPassword = true;
                    }
                }
            }

            if ( foundEmailButNotPassword ){
                response.error =  (int) UserHttpErrorCodes.WRONG_PASSWORD;
                response.message = "Failure! you entered a wrong password.";
            }

            return new OkObjectResult(response);
        }
    }
}
