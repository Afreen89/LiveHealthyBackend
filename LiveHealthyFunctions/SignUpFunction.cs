using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LiveHealthy
{
    public static class SignUpFunction
    {
        [FunctionName("SignUpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
            databaseName: "Users",
            collectionName: "users",
            ConnectionStringSetting = "cosmosDBConnectionString")]ICollector<UserProfile> documentsOut,
            ILogger log)
        {
            log.LogInformation($"Sign up API is called. Total user count: {documentsOut}");

            string pFirstName = req.Query["firstName"];
            string pLastName = req.Query["lastName"];
            string pEmail = req.Query["email"];
            string pPassword = req.Query["password"];
            string pAge = req.Query["age"];
            string pGender = req.Query["gender"];
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            pFirstName = pFirstName ?? data?.firstName;
            pLastName = pLastName ?? data?.lastName;
            pEmail = pEmail ?? data?.email;
            pPassword = pPassword ?? data?.password;
            pAge = pAge ?? data?.age;
            pGender = pGender ?? data?.gender;

            UserHttpResponse response  = new UserHttpResponse();
            response.message = "Fail: Unknown";
            response.error =  (int) UserHttpErrorCodes.UNKNOWN_ERROR;

            if (  string.IsNullOrEmpty(pFirstName) ){
                response.error =  (int) UserHttpErrorCodes.MISSING_FIRSTNAME;
                response.message = "Error: First name is missing.";
            } else if ( string.IsNullOrEmpty(pLastName ) ){
                response.error =  (int) UserHttpErrorCodes.MISSING_LASTNAME;
                response.message = "Error: Last name is missing.";
            } else if ( string.IsNullOrEmpty(pEmail ) ){
                response.error =  (int) UserHttpErrorCodes.MISSING_EMAIL;
                response.message = "Error: User email is missing.";
            } else if ( string.IsNullOrEmpty(pPassword ) ){
                response.error = (int)  UserHttpErrorCodes.MISSING_PASSOWRD;
                response.message = "Error: User password is missing.";
            } else if ( string.IsNullOrEmpty(pAge ) ){
                response.error =  (int) UserHttpErrorCodes.MISSING_AGE;
                response.message = "Error: User age is missing.";
            } else if ( string.IsNullOrEmpty(pLastName ) ){
                response.error =  (int) UserHttpErrorCodes.MISSING_GENDER;
                response.message = "Error: User gender name is not specified.";
            } else {

                // probably check if the email 
                // exists in the database here!
                UserProfile output = new UserProfile
                {
                    Id = System.Guid.NewGuid().ToString(),
                    pEmail =  pEmail,
                    pPassword = pPassword,
                    pFirstName = pFirstName,
                    pLastName = pLastName,
                    pAge = Double.Parse(pAge),
                    pGender = pGender
                };

                documentsOut.Add(output);

                response.error =  (int) UserHttpErrorCodes.SUCCESS_STATUS;
                response.message = "Success! new user has been created";
                response.data = output;
            }


            return new OkObjectResult(response);
        }
    }
}
