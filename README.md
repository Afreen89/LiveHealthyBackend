# Setup Up 

## Create a new resource group 

```bash
# create a new resource group
$ az group create --name LiveHealthy --location westus
# check the list of resource group already created.
$ az group list 

# Delete a resource group (be careful!)
$ az group delete --name exampleGroup
```


## Create a new IoT hub

```bash
# create a iot hub. Name should be globally unique
$ az iot hub create --name LiveHealthyHub --resource-group LiveHealthy --sku S1

# lookup commands from deleting IoT hub (if you wnat) or list them like resource group.
```

There are multiple things that can be done between device-cloud. Device here is an IoT device. You can either send telemetry data (such as heart rate, BP, etc. ) or you can send alerts messages using an Event Hub. An example of event could be like that a patient is connected to an IoT device and this device sends heart rate data. Now, an event might be that if the heart rate goes above certain value lets say 100, a separate event message is send to Event Hub that notifies that this specific event i.e. heart rate has gone above 100 to the IoT hub (which can be used on the front end static web app, alert the doctor or nurses etc.).

We will be creating 1 telemetry data stream from device to cloud and 3 events for heart rate, blood pressure and body temperature.

The structure of the telemetry data will be: 

```json
// I assume that each patient is connected to one simulated device. 
// each device sends the same telemetry and same events. 
// I also assume that each patient has a pre-defined profile with pEmail and pPassword
{
    pEmail : userEmail,
    pPassword : userPassword,
    pName : pName,

    pTemperature : pTemperature,
    pHeartRate : pHeartRate,
    pBloodPressure : pBloodPressure,
    pWeight : pWeight,
    pHeight : pHeight
}

// Please note that *pEmail* and *pPassword* will be used to retrieve specific patient data from the HTTPS function on the static web app. 
```

## Create a DB & Collections

I created a fake medicalrecord-cosmosdb and under this I created LiveHealthyDB. This DB will have 2 collections tables. One will hold the patients profiles i.e. *LiveHealthyDbUserProfiles* and the other will hold all the telemetry data i.e. *LiveHealthyDbUserData*.

The structure of the *LiveHealthyDbUserProfiles* will be as follows: 

```json
{
    pFirstName: FirstName,
    pLastName: LastName,
    pEmail: Email,
    pEmailConfirm: EmailConfirm,
    pPassword: Password,
    pPasswordConfirm: PasswordConfirm,
    pAge : Age,
    pGender : Gener,
}

```

- This profile will be created based on the Sign Up option on the static web app. 

- For sign in page, we will use hte email and password of an already existing account.

