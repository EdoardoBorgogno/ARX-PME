using ARX_PME_Connector.API;
using Newtonsoft.Json;
using System.Text;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -customer
    /// </summary>
    internal class CustomerActions : ICommand
    {
        //Fields
        ApiPME api;
        string customerName;
        string customerAddress;
        string customerCity;

        //
        // Constructor : set and check parameters
        //
        public CustomerActions(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";

            // Check number of parameters
            if (parameters.Length == 0) //If there is no parameters show error => ragione_sociale(name) must be present
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                // Set parameters
                customerName = parameters[0];

                // Remove name from args.
                parameters = parameters.Skip(1).ToArray();

                // Check if there is optional param
                checkOptionalParameters(parameters);
            }
        }

        //
        // Method
        //

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

            return await CustomerExecuteActions();
        }

        /// <summary>
        /// Separe work if customer already exists.
        /// </summary>
        /// <returns>True if all ended correctly</returns>
        public async Task<bool> CustomerExecuteActions()
        {
            //Variables
            bool isSuccess = false;
            int customerExists; //==> if it's -1 => customer doesn't exists

            //Check if customer exists
            customerExists = await api.getCustomerId(customerName);

            //If customer exists
            if (customerExists != -1)
            {
                //Update customer
                isSuccess = await UpdateCustomer();
            }
            else
            {
                //Create customer
                isSuccess = await CreateCustomer();
            }

            //Return
            return isSuccess;
        }

        /// <summary>
        /// Create customer with PME's Api.
        /// </summary>
        /// <returns>True if creation completed otherwise false.</returns>
        private async Task<bool> CreateCustomer()
        {
            //Variables
            bool creationCompleted = false;
            string urlApi = Config.GetApiUrl() + "/api/customer"; //Url for the request

            //Parameters
            Dictionary<string, string> requestParameters = new Dictionary<string, string> //Dictionary with parameters
            {
                { "city", customerCity == null ? "" : customerCity },
                { "company", customerName },
                { "type", "Company" },
                { "address", customerAddress == null ? "" : customerAddress }
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestParameters), Encoding.UTF8, "application/json"); //Content for the request

            //Make Request
            var response = await api.ApiRequest.PostAsync(urlApi, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                creationCompleted = true;
            }

            //return
            return creationCompleted;
        }

        /// <summary>
        /// Update customer with PME's API
        /// </summary>
        /// <returns>True if update ended correctly</returns>
        private async Task<bool> UpdateCustomer()
        {
            //Variables
            bool updateCompleted = false;
            string urlApi = Config.GetApiUrl() + "/api/customer/" + await api.getCustomerId(customerName); //Url for the request

            //Parameters
            Dictionary<string, string> requestParameters = new Dictionary<string, string> //Dictionary with parameters
            {
                { "type", "Company" }
            };

            //Add Param
            if (customerAddress != null) //Add address if it is set
                requestParameters.Add("address", customerAddress);
            if (customerCity != null) //Add city if it is set
                requestParameters.Add("city", customerCity);

            //Create httpcontent
            var content = new StringContent(JsonConvert.SerializeObject(requestParameters), Encoding.UTF8, "application/json"); //Content for the request

            //Make Request
            var response = await api.ApiRequest.PutAsync(urlApi, content);

            //If response is successful
            if(response.IsSuccessStatusCode)
            {
                updateCompleted = true;
            }

            //return
            return updateCompleted;
        }

        /// <summary>
        /// Check and set optional parameters.
        /// </summary>
        /// <param name="parameters">Array with parameters</param>
        private void checkOptionalParameters(string[] parameters)
        {
            //Variables
            bool valueIsPresent;
            string value;

            //
            // Set and check Address
            //

            //Get address Value
            valueIsPresent = ParamHandler.getParamValue(parameters, "-i", out value);

            if(valueIsPresent)
                customerAddress = value;

            //
            // Set and check City
            //

            //Get city value
            valueIsPresent = ParamHandler.getParamValue(parameters, "-c", out value);

            if (valueIsPresent)
                customerCity = value;

        }
    }
}
