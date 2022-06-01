# Setup Up 

## Resource Group 

```bash
# create a new resource group
$ az group create --name LiveHealthy --location westus
# check the list of resource group already created.
$ az group list 

# Delete a resource group (be careful!)
$ az group delete --name exampleGroup
```


## IoT Hub and Data Structure

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

## Database & Collections

I created a *cloudassignment-cosmos* and under this I created *UserData*. This DB will have 2 collections tables. One will hold the patients profiles i.e. *userprofile* and the other will hold all the telemetry data i.e. *userdata*.

The structure of the *userprofile* will be as follows: 

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

- For sign in page, we will use the email and password of an already existing account.


## Azure Function App: 

For data retrieval, I created one IoTHub Trigger and 5 Http Trigger as following: 

- *LiveHealthyFunctions.cs* - IoThub Trigger Function - Store device data to DB.
- *SignInFunction.cs* - Http Trigger - Retrieves user profile from DB when email and password is passed.
- *SignUpFunction.cs* - Http Trigger - Saves a profile for new user in DB when a new profile is passed (see UserProfile data structure).
- *RetrieveUserPassword.cs* - Http Trigger - Retrieve the user profile/password when first, last names and email is passed. 
- *GetUserData.cs* - Http Trigger - Retrieves all the user data saved by IoTHub trigger (for specific simulated device i.e. specific user) when the email and password is passed. 
- *EditUserProfile* - HttpTrigger - NOT USED! 


Please note that most of the functions are written so that they can either take Query arguments in the http-link request or take a separate request body. The HTTP code look for same arguments in both places. For static web app, the arguments/request params are passed using Query instead of a separate request body.

**Automatic Deployment Using Github Actions**

The functions are automatically deployed to Azure Function App whenever a new commit is pushed (or a new PR is merged) into the *second-subscription* branch of this repository.

## Static Web App:

The codes for static web app/client is in a separate repository (LiveHealthyClient). Please see the *second-subscription* branch instead of *main*.


The LiveHealthy static web app is written on VueJS. In addition the VueJs is using apex-chart library for graphs, Bootstrap for CSS and table, Axios to send the HTTP requests (to Azure Http Functions) and vue-router to move around the pages (from sign in to sign up and dashboard etc.)

The app is automatically deployed everytime a commit is pushed to remote repository and GitHub actions (which were pre-configured for the Azure-VueJs project) are triggered that help in deploying the web-app online. The deployment workflow can be seen on the following git-link: 

```bash
# GitHub Actions Deployment workflow: 
https://github.com/Afreen89/LiveHealthyClient/actions 
```



The web-app is live on the following link: 

```bash
# Web ap link: 
https://thankful-grass-0f213b90f.1.azurestaticapps.net/
```
