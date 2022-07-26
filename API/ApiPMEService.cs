﻿using System.Net;
using Newtonsoft.Json.Linq;

namespace ARX_PME_Connector.API
{
    /// <summary>
    /// Provides Pme's Api services.
    /// </summary>
    internal class ApiPMEService : IApi
    {
        //
        // Fields
        //

        private HttpClient apiRequest;
        private int requestCount;

        //
        // Properties
        //

        public int RequestCount { get => requestCount; }
        public HttpClient ApiRequest { get => apiRequest; }

        //Constructor
        public ApiPMEService()
        {
            //Create handler for HttpClient
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            //Initialize the HttpClient with handler
            apiRequest = new HttpClient(handler);

            //Initialize the request count
            requestCount = 0;
        }

        //
        // Methods
        //

        /// <summary>
        /// Login to Pme's Api.
        /// </summary>
        /// <returns>True if Login ended correctly</returns>
        public async Task<bool> ApiLogin()
        {
            //API documentation: https://www.planningpme.it/planningpme-api-sviluppatori-documentazione.htm
            // Pme's Api login needs an appkey.
            // This is a unique key that is generated by Pme, and is used to authenticate the application.
            // This key is located in Config.json

            //Variables
            string appKey = ""; // Appkey
            bool isLogged = false; //==> true if login end correctly

            //Get the appkey from Config.json
            appKey = Config.GetAppkey();

            //Add the appkey to the request header
            apiRequest.DefaultRequestHeaders.Add("X-APPKEY", appKey);

            //
            // Login to Api
            //

            // Get a dictionary of all useful data for the login
            Dictionary<string, string> loginData = Config.UserLoginData();

            // Make /token request
            var response = await apiRequest.PostAsync(Config.GetApiUrl() + "/token", new FormUrlEncodedContent(loginData));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Get the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // Get the token from the response content
                string token = JObject.Parse(responseContent)["access_token"]!.ToString();

                // Add the token to the request header
                apiRequest.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            //
            // Try to make request
            //

            int testCode = await ApiTest();

            if (testCode == 200) //If ApiTest return 200 
            {
                isLogged = true;
            }

            return isLogged;
        }

        /// <summary>
        /// Simple Api Call for verify connection.
        /// </summary>
        /// <returns>200 if call ended correctly otherwise -1.</returns>
        public async Task<int> ApiTest()
        {
            //Variables
            int responseCode;

            //Make Request
            var response = await apiRequest.GetAsync(Config.GetApiUrl() + "/api/category");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                responseCode = 200;
            }
            else
            {
                responseCode = -1;
            }


            //return responseCode
            return responseCode;
        }
    }
}
