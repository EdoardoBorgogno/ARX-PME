using ARX_PME_Connector.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace ARX_PME_Connector.Commands
{
    /// <summary>
    /// Class that handles the command -activities_in_range
    /// </summary>
    internal class ActivitiesInRange : ICommand
    {
        //Fields
        ApiPME api;
        DateTime startDate;
        DateTime endDate;
        string stateName;

        //
        // Constructor : set and check parameters
        //
        public ActivitiesInRange(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";
            bool valueIsPresent;
            string value;

            // If there is more or less than 2 parameters show paramError
            if (parameters.Length < 2)
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                startDate = DateTime.ParseExact(parameters[0].Replace('/', '-'), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                endDate = DateTime.ParseExact(parameters[1].Replace('/', '-'), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                //
                // Optional Param
                //

                //Get and Set state ("Conclsa", "Pianificata", ...)
                valueIsPresent = ParamHandler.getParamValue(parameters, "-state", out value);

                if (valueIsPresent)
                    stateName = value;
            }
        }

        //
        // Methods
        //

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns>List of activity</returns>
        public async Task<dynamic> Execute()
        {
            await PMEApiConnection(); //==> connect to API

            return await getActivitiesInRange();
        }

        /// <summary>
        /// Get activities filtred List.
        /// </summary>
        /// <returns>List of activity</returns>
        private async Task<List<Activity.Root>> getActivitiesInRange()
        {
            //Get activities list
            List<Activity.Root> activities = await GetActivitiesList(startDate, endDate);

            //If stateName is set
            if (stateName != null)
                activities = filterListByState(activities);

            //Return
            return activities;
        }

        /// <summary>
        /// Connect command to API.
        /// </summary>
        /// <returns>True if login is successful else false</returns>
        public async Task<bool> PMEApiConnection()
        {
            // Variables
            bool isLogged;

            //Login to Api
            api = new ApiPME();
            isLogged = await api.ApiLogin();

            // Return
            return isLogged;
        }

        /// <summary>
        /// Get the activities list from the API.
        /// </summary>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <returns></returns>
        private async Task<List<Activity.Root>> GetActivitiesList(DateTime start, DateTime end)
        {
            //Variables
            List<Activity.Root> activitiesList = new List<Activity.Root>(); //List of activities
            List<int> activitiesKeys = new List<int>(); //List of activities keys

            string urlApi = Config.GetApiUrl() + "/api/do/list"; //Url for the request
            
            Dictionary<string, string> requestParameters = new Dictionary<string, string> //Dictionary with parameters
            {
                { "dateFrom", start.ToString("yyyy-MM-dd") },
                { "dateTo", end.ToString("yyyy-MM-dd") }
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestParameters), Encoding.UTF8, "application/json"); //Content for the request

            //Make Request
            var response = await api.ApiRequest.PostAsync(urlApi, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Get the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                //For each activity in responseJson, add its key to the activitiesKeys
                foreach (JObject activity in responseJson["items"]!)
                {
                    activitiesKeys.Add(activity["key"]!.Value<int>());
                }

                //For each activity key in activitiesKeys, get the activity from the Api
                foreach (int activityKey in activitiesKeys)
                {
                    activitiesList.Add(await GetActivity(activityKey));
                }
            }
            else
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Api Error", MessageContent = response.StatusCode.ToString(), MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }

            //return
            return activitiesList;
        }

        /// <summary>
        /// Remove element (activity) where category is different from stateName.
        /// </summary>
        /// <param name="list">List of activities.</param>
        /// <returns>Filtred list.</returns>
        private List<Activity.Root> filterListByState(List<Activity.Root> list)
        {
            //Remove all activity where category.label != stateName
            list.RemoveAll(activity => activity.category.label.Split(" ", 2)[1] != stateName); //==> get the second part of the label ("1." => first; "StateName" => second)

            //Return
            return list;
        }

        /// <summary>
        /// Get the activity from the Api.
        /// </summary>
        /// <param name="activityKey">Activity key</param>
        /// <returns>Activity object</returns>
        private async Task<Activity.Root> GetActivity(int activityKey)
        {
            //Variables
            Activity.Root activity = new Activity.Root(); //Activity
            string urlApi = Config.GetApiUrl() + "/api/do/" + activityKey; //Url for the request

            //Make Request
            var response = await api.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Get the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                //From the json, get the activity
                activity = JsonConvert.DeserializeObject<Activity.Root>(responseJson.ToString())!;
            }
            else
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Api Error", MessageContent = response.StatusCode.ToString(), MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }

            //return
            return activity;
        }
    }
}
