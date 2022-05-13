using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Newfunction
{

    public class TemperatureItem
  {
    [JsonProperty("id")]
    public string Id {get; set;}
    public double Temperature {get; set;}
    public double Humidity {get; set;}
  }

    public class PatientRecord
    {
      [JsonProperty("id")]

      public string Id {get; set;}
      public string pEmail {get; set;}
      public string pPassword {get; set;}
      public string pName {get; set;}
      public double pTemperature {get; set;}
      public double pHeartRate {get; set;}
      public double pWeight {get; set;}
      public double pHeight {get; set;}
      public double pBloodPressure {get; set;}
    }


    public class NewIoT
    {
        private static HttpClient client = new HttpClient();
        
        // This IoT function takes data sent from 
        // simulated device and store it in 
        // the database on the server
        [FunctionName("NewIoT")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "AzureEventHubConnectionString")] EventData message,
        [CosmosDB(databaseName: "users_data",
                                 collectionName: "users_data",
                                 ConnectionStringSetting = "cosmosDBConnectionString")] out PatientRecord output,
                       ILogger log)
        {
            log.LogInformation($"Received data from IoT Hub: {Encoding.UTF8.GetString(message.Body.Array)}");        
            var jsonBody = Encoding.UTF8.GetString(message.Body);
            dynamic data = JsonConvert.DeserializeObject(jsonBody);
            output = new PatientRecord
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

        // This is an API endpoint. 
        // When called, it returns the patient 
        // temperature. The name of the patient 
        // is passed to it in req. 
        [FunctionName("GetPatientTemperature")]
          public static IActionResult GetPatientTemperature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patienttemperature/")] HttpRequest req,
            [CosmosDB(databaseName: "IoTData",
                      collectionName: "pRecords",
                      ConnectionStringSetting = "cosmosDBConnectionString",
                          SqlQuery = "SELECT * FROM c")] IEnumerable<PatientRecord> patientRecord,
                      ILogger log)
          {
            List<double> pTemp = new List<double>();  
            if ( !string.IsNullOrEmpty( req.Query["name"] ) ){
              foreach( PatientRecord p in patientRecord ){
                log.LogInformation($"Get Patient Names ===============>>>>>>: {p.pName}");
                if ( req.Query["name"] == p.pName ){
                  pTemp.Add( p.pTemperature );
                }
              }
            }

            return new OkObjectResult(pTemp);
          }


        // Get the list of distinct 
        // patient names. 
        [FunctionName("GetPatientsNames")]
          public static IActionResult GetPatientsNames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patientnames/")] HttpRequest req,
            [CosmosDB(databaseName: "IoTData",
                      collectionName: "pRecords",
                      ConnectionStringSetting = "cosmosDBConnectionString",
                          SqlQuery = "SELECT * FROM c")] IEnumerable<PatientRecord> patientRecord,
                      ILogger log)
          {
            List<string> pNames = new List<string>();  
            foreach( PatientRecord p in patientRecord ){
              log.LogInformation($"Get Patient Names ===============>>>>>>: {p.pName}");
              if (  !pNames.Contains( p.pName ) ){
                pNames.Add( p.pName );    
              }
            }
            return new OkObjectResult(pNames);
          }

        // get complete patient records 
        // (not a good idea as records can
        // be in millions and the response 
        // will take long.
        [FunctionName("GetPatientsRecords")]
          public static IActionResult GetPatientsRecords(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patientrecords/")] HttpRequest req,
            [CosmosDB(databaseName: "IoTData",
                      collectionName: "pRecords",
                      ConnectionStringSetting = "cosmosDBConnectionString",
                          SqlQuery = "SELECT * FROM c")] IEnumerable<PatientRecord> patientRecord,
                      ILogger log)
          {
            return new OkObjectResult(patientRecord);
          }


        [FunctionName("GetTemperature")]
          public static IActionResult GetTemperature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "temperature/")] HttpRequest req,
            [CosmosDB(databaseName: "IoTData",
                      collectionName: "Temperatures2",
                      ConnectionStringSetting = "cosmosDBConnectionString",
                          SqlQuery = "SELECT * FROM c")] IEnumerable< TemperatureItem> temperatureItem,
                      ILogger log)
          {
            log.LogInformation($"Get Temperature ===============>>>>>>: {req.Query["name"]}");        
            return new OkObjectResult(temperatureItem);
          }

        
        // Get the patient height 
        // when the patient name is 
        // passed to it.
        [FunctionName("GetHeight")]
          public static IActionResult GetHeight(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patientheight/")] HttpRequest req,
            [CosmosDB(databaseName: "IoTData",
                      collectionName: "pRecords",
                      ConnectionStringSetting = "cosmosDBConnectionString",
                          SqlQuery = "SELECT * FROM c")] IEnumerable< PatientRecord> patientRecord,
                      ILogger log)
          {
            List<double> pHeights = new List<double>();  

            if ( !string.IsNullOrEmpty( req.Query["name"] ) ) {
              foreach( PatientRecord p in patientRecord ){
                log.LogInformation($"Get Patient Heights ===============>>>>>>: {req.Query["name"]}");
                if ( req.Query["name"] == p.pName ){
                  pHeights.Add(p.pHeight);
                }
              }
            }

            
            return new OkObjectResult(pHeights);
          }
          
          
          
    
        
        
        
        
    }
}