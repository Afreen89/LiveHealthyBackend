// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using Microsoft.Azure.Devices.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimulatedDevice
{
    /// <summary>
    /// This sample illustrates the very basics of a device app sending telemetry. For a more comprehensive device app sample, please see
    /// <see href="https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/master/iot-hub/Samples/device/DeviceReconnectionSample"/>.
    /// </summary>
    internal class Program
    {
        public static string userEmail = "afreen.naz@gmail.com";
        public static string userPassword = "1234";
        
        private static DeviceClient s_deviceClient;
        private static readonly TransportType s_transportType = TransportType.Mqtt;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        // look in IoT Explroer or Azure portal under ResourceGroup->IoT-Hub->Devices
        private static string s_connectionString = "HostName=cloudassignment-hub.azure-devices.net;DeviceId=cloudassignment-iot-hub-device;SharedAccessKey=qv8ZCO0iJsRw5v7LpQRzFO2pmAqkZLimspax1n1At0E=";
		
        private static async Task Main(string[] args)
        {

            // This sample accepts the device connection string as a parameter, if present
            ValidateConnectionString(args);

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, s_transportType);

            // Set up a condition to quit the sample
            Console.WriteLine("Press control-C to exit.");
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            // Run the telemetry loop
            await SendDeviceToCloudMessagesAsync(cts.Token);

            s_deviceClient.Dispose();
            Console.WriteLine("LiveHealthy simulator finished.");
        }

        private static void ValidateConnectionString(string[] args)
        {
            if (args.Any())
            {
                try
                {
                    var cs = IotHubConnectionStringBuilder.Create(args[0]);
                    s_connectionString = cs.ToString();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error: Unrecognizable parameter '{args[0]}' as connection string.");
                    Environment.Exit(1);
                }
            }
            else
            {
                try
                {
                    _ = IotHubConnectionStringBuilder.Create(s_connectionString);
                }
                catch (Exception)
                {
                    // Console.WriteLine("This sample needs a device connection string to run. Program.cs can be edited to specify it, or it can be included on the command-line as the only parameter.");
                    Environment.Exit(1);
                }
            }
        }

        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync(CancellationToken ct)
        {
            var rand = new Random();

            // patient random data generator 
            double pTemperatureMin = 20;
            double pHeartRateMin = 72;
            double pBloodPressureMin = 150;
            double pWeightMin = 70;
            double pHeightMin = 160;
            string pName = "Arshad Khan";
            
            while (!ct.IsCancellationRequested)
            {

                // generating random data for pName-patient
                double pTemperature = pTemperatureMin + rand.NextDouble() * 15;
                double pHeartRate = pHeartRateMin + rand.NextDouble() * 25;
                double pBloodPressure = pBloodPressureMin + rand.NextDouble() * 15;
                double pWeight = pWeightMin + rand.NextDouble() * 5;
                double pHeight = pHeightMin + rand.NextDouble() * 5;

                // Create JSON message
                string messageBody = JsonSerializer.Serialize(
                    new
                    {
                        pEmail = userEmail,
                        pPassword = userPassword,
                        pName = pName,
                        pTemperature = pTemperature,
                        pHeartRate = pHeartRate,
                        pBloodPressure = pBloodPressure,
                        pWeight = pWeight,
                        pHeight = pHeight
                    });

                using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                };

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("pBodyTemperatureAlert", (pTemperature > 30) ? "true" : "false");
                message.Properties.Add("pHeartRateAlert", (pHeartRate > 100) ? "true" : "false");
                message.Properties.Add("pBloodPressureAlert", (pBloodPressure > 180) ? "true" : "false");

                // Send the telemetry message
                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending telemetry data: {messageBody}");

                await Task.Delay(10000);
            }
        }
    }
}
