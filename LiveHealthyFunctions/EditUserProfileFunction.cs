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
    public static class EditUserProfileFunction
    {
        [FunctionName("EditUserProfileFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Users",
                collectionName: "users",
                ConnectionStringSetting = "cosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")] IEnumerable<UserProfile> userProfiles,
            ILogger log)
        {
            log.LogInformation("Edit profile request called.");

            string pId = req.Query["id"];
            string pFirstName = req.Query["firstName"];
            string pLastName = req.Query["lastName"];
            string pEmail = req.Query["email"];
            string pPassword = req.Query["password"];
            string pAge = req.Query["age"];
            string pGender = req.Query["gender"];
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            pId = pId ?? data?.id;
            pFirstName = pFirstName ?? data?.firstName;
            pLastName = pLastName ?? data?.lastName;
            pEmail = pEmail ?? data?.email;
            pPassword = pPassword ?? data?.password;
            pAge = pAge ?? data?.age;
            pGender = pGender ?? data?.gender;

            UserHttpResponse response  = new UserHttpResponse();
            response.message = "Fail: Unknown";
            response.error =  (int) UserHttpErrorCodes.UNKNOWN_ERROR;

            bool editted = false;
            foreach( UserProfile user in userProfiles ){
                if ( pId == user.Id ){
                    
                    user.pFirstName = pFirstName;
                    user.pLastName = pLastName;
                    user.pEmail = pEmail;
                    user.pPassword = pPassword;
                    user.pGender = pGender;

                    response.error =  (int) UserHttpErrorCodes.SUCCESS_STATUS;
                    response.message = "Success! you profile is successfully saved.";
                    response.data = user;
                    editted = true; 
                    break;
                } 
            }

            if (!editted){
                response.message = "Fail! Unable to find this user in our databse. Wait, how did you sign in then?";
                response.error =  (int) UserHttpErrorCodes.USER_NOT_FOUND;
            }

            return new OkObjectResult(response);
        }
    }
}
