namespace ARX_PME_Connector.API
{

    /// <summary>
    /// Provides base Api's Operations.
    /// </summary>
    internal interface IApi
    {
        //
        // Riepilogo: 
        //      Gets the HttpClient object.
        // Valori restituiti:
        //      HttpClient object.
        HttpClient ApiRequest
        {
            get;
        }

        //
        // Riepilogo: 
        //      Gets the total count of api requests.
        // Valori restituiti:
        //      The number of requests. 
        int RequestCount
        {
            get;
        }

        //
        // Riepilogo:
        //      Set HttpHeaders necessary permissions.
        //      
        // Valori restituiti:
        //      true if HttpHeaders is correctly set otherwise false.
        Task<bool> ApiLogin();

        //
        // Riepilogo:
        //      Test Api call, call Api for get test response.
        // Valori restituiti:
        //      200 if call ended correctly otherwise -1.
        Task<int> ApiTest();
    }

}
