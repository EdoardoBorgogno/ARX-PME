using ARX_PME_Connector.API;
using Newtonsoft.Json;
using System.Text;

namespace ARX_PME_Connector.Commands
{
    /// <summary>
    /// Class that handles the command -update_activity_state
    /// </summary>
    internal class UpdateActivityState : ICommand
    {
        //Fields
        ApiPME api;
        string activityId;
        string state;

        //
        // Constructor : set and check parameters
        //
        public UpdateActivityState(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";
            string paramTypeError = "Params type error;";

            // If there is more or less than 2 parameters show paramError
            if (parameters.Length != 2)
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                //Set activity id
                if(!int.TryParse(parameters[0], out int id)) // if is not an integer show paramTypeError
                {
                    LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Type Error", MessageContent = paramTypeError, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);
                }
                else
                {
                    activityId = id.ToString();
                }

                //Set state
                state = parameters[1];
            }
        }

        //
        // Methods
        //

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Execute()
        {
            if (state != null && activityId != null)
            {
                await PMEApiConnection(); //==> connect to API

                return await UpActivityState();
            }
            
            return false;
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
        /// Update activity state.
        /// </summary>
        /// <returns>True if update ended correctly</returns>
        private async Task<bool> UpActivityState()
        {
            // Variables
            string noActivity = "No Activity with id: " + activityId + ". Check Activity List."; //==> error message
            string noState = "No State with name: " + state + ". Check State List."; //==> error message
            string urlApi = Config.GetApiUrl() + "/api/do/" + activityId; //Url for the request
            bool activityExists; //==> true if exists
            bool updateCompleted = false;

            // Check if activity exists
            activityExists = await CheckActivity(Convert.ToInt32(activityId));

            //If activity exists
            if (activityExists)
            {
                //If State exits stateId != -1
                int stateId = await api.GetStateId(state);
                if (stateId != -1)
                {
                    //Http request content
                    var content = new StringContent(JsonConvert.SerializeObject(new { category = new { key = stateId } }), Encoding.UTF8, "application/json");

                    //Make Put Request with content
                    var response = await api.ApiRequest.PutAsync(urlApi, content);

                    //If response is ok
                    if (response.IsSuccessStatusCode)
                    {
                        updateCompleted = true;
                    }
                    else
                    {
                        LogHandler.Message message = new LogHandler.Message { MessageTitle = "Api Error", MessageContent = response.StatusCode.ToString(), MessageDateTime = DateTime.Now };
                        LogHandler.addMessage(message);
                    }
                }
                else
                {
                    //If state doesn't exist show noState
                    LogHandler.Message message = new LogHandler.Message { MessageTitle = "Error", MessageContent = noState, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);
                }
            }
            else
            {
                //Show noActivity
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Error", MessageContent = noActivity, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }

            // Return
            return updateCompleted;
        }

        /// <summary>
        /// Check if activity exists.
        /// </summary>
        /// <param name="activityKey">Activity key</param>
        /// <returns>True if exists otherwise false</returns>
        private async Task<bool> CheckActivity(int activityKey)
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/do/" + activityKey; //Url for the request
            bool acitvityExists = false;

            //Make Request
            var response = await api.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                acitvityExists = true;
            }

            //return
            return acitvityExists;
        }
    }
}
