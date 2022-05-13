using Newtonsoft.Json;

namespace LiveHealthy
{
    public class UserData 
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

    
    public class UserProfile 
    {
        [JsonProperty("id")]
        public string Id {get; set;}
        public string pEmail {get; set;}
        public string pPassword {get; set;}
        public string pFirstName {get; set;}
        public string pLastName {get; set;}
        public double pAge {get; set;}
        public string pGender {get; set;}
    }

    public class UserHttpResponse 
    {
        public string message {get; set;}
        public int error {get; set;}
        public UserProfile data {get; set;}
    }

    enum UserHttpErrorCodes {
        SUCCESS_STATUS = 0,
        MISSING_EMAIL = -1,
        MISSING_PASSOWRD = -2,
        MISSING_FIRSTNAME = -3,
        MISSING_LASTNAME = -4,
        MISSING_AGE = -5,
        MISSING_GENDER = -6,
        EMAIL_ALREADY_EXIST = -7,
        WRONG_PASSWORD = -8,
        USER_NOT_FOUND = -9,
        UNKNOWN_ERROR = -10,
        
    }
}