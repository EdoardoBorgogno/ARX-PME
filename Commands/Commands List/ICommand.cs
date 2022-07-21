namespace ARX_PME_Connector.Commands
{
    /// <summary>
    /// Provides base Command Operations.
    /// </summary>
    internal interface ICommand
    {

        //
        // Riepilogo:
        //    Execute the command
        // Valori restituiti:
        //      Command result
        Task<dynamic> Execute();
    }
}
