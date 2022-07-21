using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ARX_PME_Connector
{
    /// <summary>
    /// Reader Config.json file.
    /// </summary>
    static class Config
    {
        //
        // Properties
        //

        // apiPmeNode contains json node "ApiPME"
        private static JObject apiPmeNode { get; set; }

        // legacyNode contains json node "LegacyVersion"
        private static JObject legacyVersionNode { get; set; }

        // updaterNode contains json node "updater"
        private static JObject updaterNode { get; set; }

        //
        // Constructor 
        //

        static Config()
        {
            apiPmeNode = ReadApiPME();
            legacyVersionNode = ReadLegacyVersion();
            updaterNode = ReadUpdate();
        }


        //
        // Methods node "Update"
        //

        /// <summary>
        /// Read all data of updater node from Config.json.
        /// </summary>
        /// <returns>Update JObject.</returns>
        private static JObject ReadUpdate()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json"; ;

            // Read config.json file
            string json = File.ReadAllText(path);

            // Convert json to object
            JObject configObj = JObject.Parse(json);

            // Return ApiPME node as JObject
            return (JObject)configObj["updater"]!;
        }

        /// <summary>
        /// Set value of property autoupdate.
        /// </summary>
        /// <param name="autoupdate">Bool Value</param>
        public static void setAutoupdate(bool? autoupdate)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json"; //==> get path

            //Read json config
            JObject json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path))!;

            //Change json value
            json["updater"]!["autoUpdate"] = autoupdate;

            //Write text
            File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
        }


        /// <summary>
        /// Get the value of local version.
        /// </summary>
        /// <returns>local version.</returns>
        public static string getVersion()
        {
            //Get updater node
            JObject? node = updaterNode;

            //Get version
            string param;

            param = node["localVersion"]!.ToString();

            //Return param
            return param;
        }

        /// <summary>
        /// Update node updater.
        /// </summary>
        /// <param name="time">When task execute.</param>
        /// <param name="every">Repetition every X days.</param>
        public static void updateSchedulerData(string time, int every)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json"; //==> get path

            //Read json config
            JObject json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path))!;

            //Change json value
            json["updater"]!["installUpdateAt"] = time;
            json["updater"]!["everyXDays"] = every;

            //Write text
            File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        //
        // Methods node "LegacyVersion"
        //


        /// <summary>
        /// Read all data of LegacyVersion node from Config.json.
        /// </summary>
        /// <returns>LegacyVersion JObject.</returns>
        private static JObject ReadLegacyVersion()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/";

            // Read config.json file
            string json = File.ReadAllText(path + "Config.json");

            // Convert json to object
            JObject configObj = JObject.Parse(json);

            // Return ApiPME node as JObject
            return (JObject)configObj["LegacyVersion"]!;
        }

        /// <summary>
        /// Get the value of attività_da_leggere from Config.json.
        /// </summary>        
        /// <returns>attività_da_leggere string.</returns>
        public static string? GetAttività_da_leggereRCommand()
        {
            //Get LegacyVersion node
            JObject legacyVersionObj = legacyVersionNode;

            //Get param attività_da_leggere
            string? param = legacyVersionObj["commands"]!["-r"]!["attività_da_leggere"]!.ToString();

            //Return param
            return param;
        }

        /// <summary>
        /// Get the value of disable_name from Config.json.
        /// </summary>
        /// <returns>disable project resource name.</returns>
        public static string? GetDisableResourceName()
        {
            //Get LegacyVersion node
            JObject legacyVersionObj = legacyVersionNode;

            //Get param attività_da_leggere
            string? param = legacyVersionObj["commands"]!["-disable_project"]!["disable_name"]!.ToString();

            //Return param
            return param;
        }

        /// <summary>
        /// Get the value of cancel_name from Config.json.
        /// </summary>
        /// <returns>cancel project resource name.</returns>
        public static string? GetCancelResourceName()
        {
            //Get LegacyVersion node
            JObject legacyVersionObj = legacyVersionNode;

            //Get param attività_da_leggere
            string? param = legacyVersionObj["commands"]!["-cancel_project"]!["cancel_name"]!.ToString();

            //Return param
            return param;
        }

        /// <summary>
        /// Get the value of element with key = name.
        /// </summary>
        /// <param name="name">Name of state_key</param>
        /// <returns></returns>
        public static string? GetStateNameUACommand(string name)
        {
            //Get LegacyVersion node
            JObject legacyVersionObj = legacyVersionNode;

            //Get params
            JObject uaParams = legacyVersionObj["commands"]!["-ua"]! as JObject;
            
            if(uaParams!.ContainsKey(name))
                return uaParams.GetValue(name)!.ToString();
            
            return null;
        }


        //
        // Methods node "ApiPME"
        //


        /// <summary>
        /// Read all data of ApiPME node from Config.json.
        /// </summary>
        /// <returns>ApiPME JObject.</returns>
        private static JObject ReadApiPME()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/";

            // Read config.json file
            string json = File.ReadAllText(path + "Config.json");
            
            // Convert json to object
            JObject configObj = JObject.Parse(json);

            // Return ApiPME node as JObject
            return (JObject)configObj["ApiPME"]!;
        }

        /// <summary>
        /// Get the value of Appkey from Config.json.
        /// </summary>        
        /// <returns>Appkey string.</returns>
        public static string GetAppkey()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get Appkey
            string appKey = apiPmeObj["appkey"]!.ToString();

            //Return Appkey
            return appKey;
        }

        /// <summary>
        /// Get the value of ApiUrl from Config.json.
        /// </summary>
        /// <returns>ApiUrl string.</returns>
        public static string GetApiUrl()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get ApiUrl
            string apiUrl = apiPmeObj["url"]!.ToString();

            //Return ApiUrl
            return apiUrl;
        }

        /// <summary>
        /// Get the value of jwt token from Config.json.
        /// </summary>
        /// <returns>jwt token string.</returns>
        public static string GetJwtToken()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get jwt token
            string jwtToken = apiPmeObj["userLogin"]!["jwt"]!.ToString();

            //Return jwt token
            return jwtToken;
        }

        /// <summary>
        /// Get the loginType from Config.json.
        /// </summary>
        /// <returns>LoginType string.</returns>
        public static string GetLoginType()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get loginType
            string loginType = apiPmeObj["userLogin"]!["loginType"]!.ToString();

            //Return loginType
            return loginType;
        }

        /// <summary>
        /// Get the value of username from Config.json.
        /// </summary>
        /// <returns>Username string.</returns>
        public static string GetUsername()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get username
            string username = apiPmeObj["userLogin"]!["username"]!.ToString();

            //Return username
            return username;
        }

        /// <summary>
        /// Get the value of password from Config.json.
        /// </summary>
        /// <returns>Password string.</returns>
        public static string GetPassword()
        {
            //Get ApiPME node
            JObject apiPmeObj = apiPmeNode;

            //Get password
            string password = apiPmeObj["userLogin"]!["password"]!.ToString();

            //Return password
            return password;
        }

        /// <summary>
        /// Get the data for user login from Config.json.
        /// </summary>
        /// <returns>Dictionary with useful datas</returns>
        public static Dictionary<string, string> UserLoginData()
        {
            //Dictionary with useful datas
            Dictionary<string, string> userLoginData = new Dictionary<string, string>();

            if (GetLoginType() == "jwt") //if loginType is "jwt"
            {
                //Get jwt token
                string jwtToken = GetJwtToken();
                
                //Add grant to dictionary
                userLoginData.Add("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");

                //Add jwt token to dictionary
                userLoginData.Add("assertion", jwtToken);
            }
            else //if loginType is "basic"
            {
                //Get username
                string username = GetUsername();

                //Get password
                string password = GetPassword();

                //Add grant_type to dictionary
                userLoginData.Add("grant_type", "password");

                //Add username and password to dictionary
                userLoginData.Add("username", username);
                userLoginData.Add("password", password);
            }

            //Return userLoginData
            return userLoginData;
        }

    }
}