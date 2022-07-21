using Newtonsoft.Json.Linq;

namespace ARX_PME_Connector.API
{
    /// <summary>
    /// Provides method for execute most frequently request and Pme's Api login.
    /// </summary>
    internal class ApiPME : ApiPMEService
    {
        //
        // PME frequently request
        //

        //
        // Provides method for execute most frequently request
        //

        /// <summary>
        /// Get customer id from Customer Name.
        /// </summary>
        /// <returns>Int with id, if customer doesn't exists or 
        /// call failed return -1.</returns>
        public async Task<int> getCustomerId(string customerName)
        {
            //Variable
            int customerId = -1;
            string urlApi = Config.GetApiUrl() + "/api/customer?name=" + customerName; //Url for the request

            //Make Request
            var response = await this.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                //If there is almost one items
                if (responseJson["items"]!.Count() > 0)
                {
                    //Get Company
                    string company = (string)responseJson["items"]![0]!["company"]!;

                    //If company is equal to customerName
                    if (company == customerName)
                    {
                        //Set customerid to value
                        customerId = Convert.ToInt32(responseJson["items"]![0]!["key"]);
                    }
                }
            }

            //return 
            return customerId;
        }

        /// <summary>
        /// Get project id from project Name and customer Name.
        /// </summary>
        /// <returns>Int that represents id, if project doesn't exist -1</returns>
        public async Task<int> getProjectId(string projectName, string customerName)
        {
            // Variables
            int projectId = -1;
            string urlApi = Config.GetApiUrl() + "/api/project?customers=" + await getCustomerId(customerName); //Url for the request

            // Make Request
            var response = await this.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                // If there is almost one items
                if (responseJson["items"]!.Count() > 0)
                {
                    //Foreach item in items
                    foreach (var item in responseJson["items"]!)
                    {
                        //If there is a project with the same name
                        if (item["label"]!.ToString() == projectName)
                        {
                            //exit from foreach and return true
                            projectId = Convert.ToInt32(item["key"]);
                            break;
                        }
                    }
                }
            }

            //return
            return projectId;
        }

        /// <summary>
        /// Get customer Name from Project Name.
        /// FOR LEGACY VERSION, in new version project name isn't unique.
        /// </summary>
        /// <param name="projectName">Name of project.</param>
        /// <returns>Name of Customer.</returns>
        public async Task<string> getCustomerName(string projectName)
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/project?name=" + projectName; //Url for the request
            string customerName = ""; //==> where put return value

            //Make request
            var response = await this.ApiRequest.GetAsync(urlApi);

            // If response is successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                //If there is almost one items
                if (responseJson["items"]!.Count() > 0)
                {
                    // Get customer Name
                    customerName = responseJson["items"]![0]!["customer"]!["label"]!.ToString();
                }
            }

            // Return
            return customerName;
        }

        /// <summary>
        /// Get id of resources from name.
        /// </summary>
        /// <param name="resourcesName">Name of resource</param>
        /// <returns>Int that represents id, if project doesn't exist -1</returns>
        public async Task<int> getResourceId(string resourcesName)
        {
            //Variables
            int resourcesId = -1;
            string urlApi = Config.GetApiUrl() + "/api/resource?name=" + resourcesName; //Url for the request

            // Make Request
            var response = await this.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                // If there is almost one items
                if (responseJson["items"]!.Count() > 0)
                {
                    //Get Name
                    string name = (string)responseJson["items"]![0]!["label"]!;

                    //If company is equal to customerName
                    if (name == resourcesName)
                    {
                        //Set resourceid to value
                        resourcesId = Convert.ToInt32(responseJson["items"]![0]!["key"]);
                    }
                }
            }

            //Return 
            return resourcesId;
        }

        /// <summary>
        /// Get state id from state name.
        /// </summary>
        /// <returns>State id, if state exits, else -1</returns>
        public async Task<int> GetStateId(string stateName)
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/category?name=" + stateName; //Url for the request
            int stateId = -1;

            //Make Request
            var response = await this.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                //If there is almost one items
                if (responseJson["items"]!.Count() > 0)
                {
                    //Get label
                    string label = (string)responseJson["items"]![0]!["label"]!;

                    //If label is the same as the state
                    string splitted = label.Split(' ', 2)[1]; //==> get the second part of the label ("1." => first; "StateName" => second)
                    if (splitted == stateName) // ==> label is splitted because => "3. Conclusa" (example of state label)
                    {
                        stateId = Convert.ToInt32(responseJson["items"]![0]!["key"]!); //==> get the key of the state
                    }
                }
            }

            //return
            return stateId;
        }
    }
}
