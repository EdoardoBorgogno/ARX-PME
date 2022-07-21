using ARX_PME_Connector.API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -project.
    /// </summary>
    internal class ProjectActions : ICommand
    {
        //
        // Fields
        //

        ApiPME api;
        string projectName;
        string customerName;
        string streamAdd;
        string streamDelete;
        Tuple<string, string> streamUpdate; //==> item 1 is oldStreamName / item 2 is newStreamName

        // Dictionary with "fields" => Json Node
        // Using tupla => first item is paramName in args, second item is paramName in json
        Dictionary<(string, string), string?> fields = new Dictionary<(string, string), string?>()
        {
            { ("-projectOwner", "OWNERCOMMESSA"), null },
            { ("-resources", "RISORSESUGGERITE"), null },
            { ("-customerRef", "REFCLIENTE"), null },
            { ("-internalRef", "REFAZIENDA"), null },
            { ("-expectedDays", "GGPREVISTE"), null },
            { ("-startDate", "DATAINIZIOCOMM"), null },
            { ("-endDate", "VINCOLOFINECOMM"), null },
            { ("-type", "TIPOCOMM"), null },
        };

        //
        // Constructor : set and check parameters
        //

        public ProjectActions(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";

            // If there is less than 2 parameters show paramError (projectName and customerName are mandatory)
            if (parameters.Length < 2)
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                // Set projectName
                projectName = parameters[0];

                // Set customerName
                customerName = parameters[1];

                //Remove customerName and projectName from parameters array
                parameters.Skip(1).ToArray();
                parameters.Skip(1).ToArray();

                // Check if there is optional param
                checkOptionalParameters(parameters);
            }
        }

        //
        // Methods
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

            return await ProjectExecuteActions();
        }

        /// <summary>
        /// Separe work if project already exists.
        /// </summary>
        /// <returns>True if all ended correctly</returns>
        public async Task<bool> ProjectExecuteActions()
        {
            //Variables
            bool isSuccess = false;
            int projectExists; //==> -1 if project doesn't exists

            //Check if customer exists
            projectExists = await api.getProjectId(projectName, customerName);

            //If customer exists
            if (projectExists != -1)
            {
                //Update customer
                isSuccess = await UpdateProject();
            }
            else
            {
                //Create customer
                isSuccess = await CreateProject();
            }

            //Return
            return isSuccess;
        }

        /// <summary>
        /// Create project with PME's Api.
        /// </summary>
        /// <returns>True if creation completed otherwise false.</returns>
        private async Task<bool> CreateProject()
        {
            //Variables
            bool creationCompleted = false;
            string urlApi = Config.GetApiUrl() + "/api/project"; //Url for the request

            //
            // Parameters
            //

            //Fields ==> Create new Dictionary with fields(key: fields.key.item2, value: fields[key.item2])
            Dictionary<string, string> requestFields = new Dictionary<string, string>();

            //Foreach key in array
            foreach ((string, string) key in fields.Keys)
            {
                //Add value if it's not null
                if(fields[(key.Item1, key.Item2)]! != null)
                    requestFields.Add(key.Item2, fields[(key.Item1, key.Item2)]!);
            }

            //
            // Replace '/' with '-' and Add T00:00:00 => example: 2022-06-27T00:00:00 (without T:00:00:00, API ERROR)
            //

            if (requestFields.ContainsKey("VINCOLOFINECOMM"))
            {
                requestFields["VINCOLOFINECOMM"] += "T00:00:00";
                requestFields["VINCOLOFINECOMM"] = requestFields["VINCOLOFINECOMM"].Replace('/', '-');
            }

            if (requestFields.ContainsKey("DATAINIZIOCOMM"))
            {
                requestFields["DATAINIZIOCOMM"] += "T00:00:00";
                requestFields["DATAINIZIOCOMM"] = requestFields["DATAINIZIOCOMM"].Replace('/', '-');
            }

            //Dictionary with request message
            Dictionary<string, object> requestParameters = new Dictionary<string, object> //Dictionary with parameters
            {
                { "allDepartments", true},
                { "customer", new { key = await api.getCustomerId(customerName) } },
                { "label", projectName },
                { "notValid", false },
                { "fields", requestFields }
            };

            // If subproject is set
            if (streamAdd != null)
            {
                //Create list for json ==> [{object1}, {object2}]
                List<object> subprojects = new List<object>();
                subprojects.Add(new { key = 1, label = streamAdd });

                //Add list to requestParameters
                requestParameters.Add("subProjects", subprojects);
            }

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
        /// Update project with PME's API
        /// </summary>
        /// <returns>True if update ended correctly</returns>
        private async Task<bool> UpdateProject()
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/project/" + await api.getProjectId(projectName, customerName); //Url for the request
            bool updateCompleted = false;

            //
            // Parameters
            //

            //Fields ==> Create new Dictionary with fields(key: fields.key.item2, value: fields[key.item2])
            Dictionary<string, string> requestFields = new Dictionary<string, string>();

            //Foreach key in array
            foreach ((string, string) key in fields.Keys)
            {
                //Add value if it's not null
                if (fields[(key.Item1, key.Item2)]! != null)
                    requestFields.Add(key.Item2, fields[(key.Item1, key.Item2)]!);
            }

            //
            // Replace '/' with '-' and Add T00:00:00 => example: 2022-06-27T00:00:00 (without T:00:00:00, API ERROR)
            //

            if (requestFields.ContainsKey("VINCOLOFINECOMM"))
            {
                requestFields["VINCOLOFINECOMM"] += "T00:00:00";
                requestFields["VINCOLOFINECOMM"] = requestFields["VINCOLOFINECOMM"].Replace('/', '-');
            }

            if (requestFields.ContainsKey("DATAINIZIOCOMM"))
            {
                requestFields["DATAINIZIOCOMM"] += "T00:00:00";
                requestFields["DATAINIZIOCOMM"] = requestFields["DATAINIZIOCOMM"].Replace('/', '-');
            }


            //Dictionary with request message
            Dictionary<string, object> requestParameters = new Dictionary<string, object> //Dictionary with parameters
            {
                { "fields", requestFields }
            };

            //Create list for json ==> [{object1}, {object2}]
            List<object> subprojects = new List<object>();

            //Load all subprojects
            subprojects = await loadSubProjects();

            // If subproject to add is set
            if (streamAdd != null)
                subprojects.Add(new { key = 1, label = streamAdd });

            //Add list to requestParameters
            requestParameters.Add("subProjects", subprojects);

            //Create httpcontent
            var content = new StringContent(JsonConvert.SerializeObject(requestParameters), Encoding.UTF8, "application/json"); //Content for the request

            //Make Request
            var response = await api.ApiRequest.PutAsync(urlApi, content);

            //If response is successful
            if (response.IsSuccessStatusCode)
            {
                updateCompleted = true;
            }

            //return
            return updateCompleted;
        }

        /// <summary>
        /// Load all subprojects (stream) of this project
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> loadSubProjects()
        {
            //Variables
            string urlApi = Config.GetApiUrl() + "/api/project/" + await api.getProjectId(projectName, customerName); //Url for the request
            List<object> list= new List<object>();

            //Make request
            var response = await api.ApiRequest.GetAsync(urlApi);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                // From the response content, get the json
                JObject responseJson = JObject.Parse(responseContent);

                // If there is almost 1 subproject
                if(responseJson.ContainsKey("subProjects"))
                    //Foreach subproject in subProjects
                    foreach (var subproject in responseJson["subProjects"]!)
                    {
                        if (streamUpdate != null && streamUpdate.Item1 == subproject["label"]!.ToString()) // if subproject isn't subproject to updae
                            list.Add(new { key = subproject["key"], label = streamUpdate.Item2 });
                        else if (streamDelete == null || subproject["label"]!.ToString() != streamDelete) // if subproject isn't subproject to delete
                            list.Add(new { key = subproject["key"], label = subproject["label"] });
                    }
            }

            //return
            return list;
        }

        /// <summary>
        /// Check and set optional parameters.
        /// </summary>
        /// <param name="parameters">Array with parameters</param>
        private void checkOptionalParameters(string[] parameters)
        {
            //Variables
            bool valueIsPresent;
            string secondValue; //==> use this if there is more than one value to set
            string value;

            //
            // Set and check Stream Add
            //

            valueIsPresent = ParamHandler.getParamValue(parameters, "-addStream", out value);

            if (valueIsPresent)
                streamAdd = value; //==> set value

            //
            // Set and check Stream Delete
            //

            valueIsPresent = ParamHandler.getParamValue(parameters, "-deleteStream", out value);

            if (valueIsPresent)
                streamDelete = value; //==> set value

            //
            // Set and check Stream Update
            //

            if (ParamHandler.getParamValue(parameters, "-newStreamName", out secondValue) && ParamHandler.getParamValue(parameters, "-oldStreamName", out value))
                streamUpdate = new Tuple<string, string>(value, secondValue);
            

            // 
            // Set fields 
            //

            //Foreach key in dictionary
            foreach ((string, string) key in fields.Keys)
            {
                valueIsPresent = ParamHandler.getParamValue(parameters, key.Item1, out value);

                //If param exists
                if (valueIsPresent)
                    fields[(key.Item1, key.Item2)] = value; //==> set value
            }
        }
    }
}
