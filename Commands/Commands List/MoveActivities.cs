using ARX_PME_Connector.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -move
    /// </summary>
    internal class MoveActivities : ICommand
    {
        //Fields
        ApiPME api;

        //Dictionary with httpMessage parameters
        Dictionary<string, string?> httpParameters = new Dictionary<string, string?>()
        {
            { "-withState", null }, //==> need to be first: if during params setting customerName is null, program need to get this value
            { "-projectName", null },
            { "-moveFrom", null },
            { "-customerName", null },
            { "-moveTo", null },
            { "-dateFrom", null },
            { "-dateTo", null } //==> need to be last param in dictionary: if during params setting something 
                                //    go wrong dateTo isn't set and api request will crash.
        };

        //
        // Constructor : set and check parameters
        //

        public MoveActivities(string[] parameters)
        {
            // Variables
            string paramError = "Mandatory parameters are not set. Check documentation.";
            bool paramIsPresent;
            string paramValue;

            //
            // Set all param
            //

            //foreach param in dictionary
            foreach(var param in httpParameters)
            {
                paramIsPresent = ParamHandler.getParamValue(parameters, param.Key, out paramValue); //==> get param value

                //If value exists
                if (paramIsPresent)
                    httpParameters[param.Key] = paramValue; //==> set dictionary
                else if (param.Key == "-customerName")
                {
                    if (httpParameters["-withState"] == null) //==> if -withState is set, customerName can be null
                    {
                        // Log error
                        LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                        LogHandler.addMessage(message);
                        break;
                    }
                }
                else if (param.Key != "-projectName" && param.Key != "-withState" && param.Key != "-moveFrom") // If mandatory param isn't set
                {
                    // Log error
                    LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);
                    break;
                }
            }
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
        /// Execute the command
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Execute()
        {
            await PMEApiConnection(); //==> connect to API

            return await ExecuteMove();
        }

        /// <summary>
        /// Execute command.
        /// </summary>
        /// <returns>True if movement end correctly otherwise
        /// false.</returns>
        private async Task<bool> ExecuteMove()
        {
            //
            // Get Activities List
            //

            List<int> keys = await GetActivitiesKeyList();

            //
            // Update Activities 
            //

            return await UpdateActivityResources(keys);
        }

        /// <summary>
        /// Update all activities resources node.
        /// </summary>
        /// <param name="list">List of int, activities key</param>
        /// <returns>True if update ended correctly else false</returns>
        private async Task<bool> UpdateActivityResources(List<int> list)
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/do/"; //Url for the request
            int moveToId = await api.getResourceId(httpParameters["-moveTo"]!);
            bool updateEndCorrectly = true;

            //Foreach key in list
            foreach(int key in list)
            {
                //Create url api
                string urlApiId = urlApi + key.ToString();

                //Dictionary with request message
                Dictionary<string, object> requestParameters = new Dictionary<string, object> //Dictionary with parameters
                {
                    { "resources", new List<object> { new { key = moveToId } } }
                };

                //Create httpcontent
                var content = new StringContent(JsonConvert.SerializeObject(requestParameters), Encoding.UTF8, "application/json"); //Content for the request

                //Make request
                var response = await api.ApiRequest.PutAsync(urlApiId, content);

                //If response isn't successful
                if (!response.IsSuccessStatusCode)
                {
                    updateEndCorrectly = false;
                }
            }

            //Return
            return updateEndCorrectly;
        }

        /// <summary>
        /// Get the activities key list from the API (filtred by parameters).
        /// </summary>
        /// <returns>List of int (activity key)</returns>
        private async Task<List<int>> GetActivitiesKeyList()
        {
            //Variables
            List<int> activitiesKeys = new List<int>(); //List of activities keys

            string urlApi = Config.GetApiUrl() + "/api/do/list"; //Url for the request

            Dictionary<string, object> requestParameters = new Dictionary<string, object> //Dictionary with parameters
            {
                { "dateFrom", httpParameters["-dateFrom"]! },
                { "dateTo", httpParameters["-dateTo"]! },
                { "resources", httpParameters["-moveFrom"]! != null ? new List<int> { await api.getResourceId(httpParameters["-moveFrom"]!) } : null! },
                { "customers", httpParameters["-customerName"]! != null ? new List<int> { await api.getCustomerId(httpParameters["-customerName"]!) } : null! },
                { "projects", httpParameters["-projectName"] != null ? new List<int> { await api.getProjectId(httpParameters["-projectName"]!, httpParameters["-customerName"]!) } : null! },
                { "categories", httpParameters["-withState"] != null ? new List<int> { await api.GetStateId(httpParameters["-withState"]!)} : null! }
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
            }

            //Return
            return activitiesKeys;
        }
    }
}
