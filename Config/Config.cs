using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ARX_PME_Updater
{
    /// <summary>
    /// Reader Config.json file.
    /// </summary>
    internal static class Config
    {
        //
        // Properties
        //

        // updateNode contains json node "updater"
        private static JObject? updateNode { get; set; }

        //
        // Constructor 
        //

        static Config()
        {
            // This .exe work also as installer, 
            // during installation Config file isn't present so 
            // can't read it.
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json"; //==> get path
            bool fileExists = File.Exists(path); 

            //If file exists read it
            if (fileExists)
                updateNode = ReadUpdate();
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
        /// Set new tag version.
        /// </summary>
        public static void setVersion(string tag)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json"; //==> get path

            //Read json config
            JObject json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path))!;

            //Change json value
            json["updater"]!["localVersion"] = tag;

            //Write text
            File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        /// <summary>
        /// Get installUpdateAt value.
        /// </summary>
        public static string getUpdateTime()
        {
            //Get updater node
            JObject? node = updateNode;

            //Get installUpdateAt
            string param;

            param = node["installUpdateAt"]!.ToString();

            //Return param
            return param;
        }

        /// <summary>
        /// Get the value of local version.
        /// </summary>
        /// <returns>local version.</returns>
        public static string getVersion()
        {
            //Get updater node
            JObject? node = updateNode;

            //Get version
            string param;

            // This .exe work also as installer, 
            // during installation Config file isn't present so 
            // set basical version
            if (node == null)
                param = "v.0.0.0.0";
            else
                param = node["localVersion"]!.ToString();

            //Return param
            return param;
        }
    }
}
