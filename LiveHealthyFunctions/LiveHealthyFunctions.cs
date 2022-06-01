using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;


namespace LiveHealthy
{

    public class LiveHealthyFunctions
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("LiveHealthyFunctions")]
        public void Run([IoTHubTrigger("messages/events", Connection = "AzureEventHubConnectionString")]EventData message, 
        [CosmosDB( 
            databaseName: "UserData",
            collectionName: "userdata",
            ConnectionStringSetting = "cosmosDBConnectionString")] out UserData output,
            ILogger log
        )
        {
            log.LogInformation($"Received telemtry data from simulated device: {Encoding.UTF8.GetString(message.Body.Array)}");
            var jsonBody = Encoding.UTF8.GetString(message.Body);
            dynamic data = JsonConvert.DeserializeObject(jsonBody);
            output = new UserData
            {
                pEmail = data.pEmail,
                pPassword = data.pPassword,
                pName = data.pName,
                pTemperature = data.pTemperature,
                pHeartRate = data.pHeartRate,
                pBloodPressure = data.pBloodPressure,
                pWeight = data.pWeight,
                pHeight = data.pHeight,
            };
        }
    }
}