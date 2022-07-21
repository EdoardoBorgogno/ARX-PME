using ARX_PME_Connector.API;
using Newtonsoft.Json.Linq;

namespace ARX_PME_Connector.Commands
{
    /// <summary>
    /// Convert legacy args array into new args version.
    /// </summary>
    internal static class LegacyHandler
    {
        /// <summary>
        /// Transform legacy args array into new args array.
        /// </summary>
        /// <param name="legacyArgs"></param>
        public static async Task<string[]> FromLegacyToNew(string[] legacyArgs)
        {
            //Variables
            string legacyCommand = legacyArgs[0]; //==> get legacy command

            //Swith on legacy command
            switch(legacyCommand)
            {
                case "-r": //==> in new version is -activities_in_range
                    legacyArgs[0] = "-activities_in_range";

                    paramActivitiesInRange(ref legacyArgs);

                    break;
                case "-c": //==> in new version is -customer
                    legacyArgs[0] = "-customer";

                    paramCustomer(ref legacyArgs);

                    break;
                case "-p": //==> in new version is -project
                    legacyArgs[0] = "-project";

                    paramProject(ref legacyArgs);

                    break;
                case "-ua": //==> in new version is -update_activity_state
                    legacyArgs[0] = "-update_activity_state";

                    paramUpdateActivityState(ref legacyArgs);

                    break;
                case "-disable_project": //==> in new version is -move
                    legacyArgs[0] = "-move";

                    legacyArgs = await paramMove(legacyArgs, "-disable_project");

                    break;
                case "-cancel_project": //==> in new version is -move
                    legacyArgs[0] = "-move";

                    legacyArgs = await paramMove(legacyArgs, "-cancel_project");

                    break;
                default:
                    //Command is not legacy

                    break;
            }

            //Return
            return legacyArgs;
        }

        /// <summary>
        /// Legacy param into new param.
        /// </summary>
        /// <param name="legacyArgs">Args Array</param>
        /// <param name="command">String with name of mapped command.</param>
        private static async Task<string[]> paramMove(string[] legacyArgs, string command)
        {
            //Int number of call to addParamName
            int calledTime = 0;

            //
            // Param projectName ==> add -projectName
            //

            ParamHandler.addParamName(ref legacyArgs, "-projectName", 1, ref calledTime);

            //
            // Param dateFrom ==> add -dateFrom
            //

            ParamHandler.addParamName(ref legacyArgs, "-dateFrom", 2, ref calledTime);

            //
            // Param dateFrom ==> add -dateFrom
            //

            ParamHandler.addParamName(ref legacyArgs, "-dateTo", 3, ref calledTime);

            //
            // Add Param and value for customer if it isn't already set
            //

            string value; //==> value of customer
            if(!ParamHandler.getParamValue(legacyArgs, "-customerName", out value)) //==> if there is no value for -customerName
            {
                //Api Object for get customer name
                ApiPME api = new ApiPME();

                //Login to api
                await api.ApiLogin();

                // Resize Array ==> need to add two element
                Array.Resize(ref legacyArgs, legacyArgs.Length + 2);

                // Add param name
                legacyArgs[legacyArgs.Length - 2] = "-customerName";

                // Add param value
                legacyArgs[legacyArgs.Length - 1] = await api.getCustomerName(legacyArgs[2]);
            }

            //
            // Add Param moveTo
            //

            // Resize Array ==> need to add two element
            Array.Resize(ref legacyArgs, legacyArgs.Length + 2);

            // Add param name
            legacyArgs[legacyArgs.Length - 2] = "-moveTo";

            // Add param value

            //If for decide wich value get from config
            if (command == "-disable_project")
                legacyArgs[legacyArgs.Length - 1] = Config.GetDisableResourceName()!;
            else
                legacyArgs[legacyArgs.Length - 1] = Config.GetCancelResourceName()!;

            //Return
            return legacyArgs;
        }

        /// <summary>
        /// Legacy param into new param.
        /// </summary>
        /// <param name="legacyArgs">Args Array</param>
        private static void paramUpdateActivityState(ref string[] legacyArgs)
        {
            //
            // State Param
            //

            string? stateValue = Config.GetStateNameUACommand(legacyArgs[2]);

            // If stateValue is set
            if (stateValue != null)
            {
                //Add to array
                legacyArgs[legacyArgs.Length - 1] = stateValue; //==> state param 
            }
        }

        /// <summary>
        /// Legacy param into new param.
        /// </summary>
        /// <param name="legacyArgs">Args Array</param>
        private static void paramProject(ref string[] legacyArgs)
        {
            //
            // Param ragione_sociale ==> remove -c
            //

            legacyArgs = legacyArgs.Where(x => x != "-c").ToArray();

            //
            // Param Stream ==> change -s into -stream
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-s") x = "-stream"; return x; }).ToArray();

            //
            // Param resources => change -rs into -resources
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-rs") x = "-resources"; return x; }).ToArray();

            //
            // Param projectOwner => change -o into -projectOwner
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-o") x = "-projectOwner"; return x; }).ToArray();

            //
            // Param customerRef => change -rc into -customerRef
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-rc") x = "-customerRef"; return x; }).ToArray();

            //
            // Param internalRef => change -ra into -internalRef
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-ra") x = "-internalRef"; return x; }).ToArray();

            //
            // Param expectedDays => change -gp into -expectedDays
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-gp") x = "-expectedDays"; return x; }).ToArray();

            //
            // Param startDate => change -di into -startDate
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-di") x = "-startDate"; return x; }).ToArray();

            //
            // Param endDate => change -dt into -endDate
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-dt") x = "-endDate"; return x; }).ToArray();

            //
            // Param type => change -t into -type
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-t") x = "-type"; return x; }).ToArray();
        }

        /// <summary>
        /// Legacy param into new param.
        /// </summary>
        /// <param name="legacyArgs">Args Array</param>
        private static void paramCustomer(ref string[] legacyArgs)
        {
            //
            // Param City ==> change -ct into -c
            //

            legacyArgs = legacyArgs.Select(x => { if (x == "-ct") x = "-c"; return x; }).ToArray();
        }

        /// <summary>
        /// Legacy param into new param.
        /// </summary>
        /// <param name="legacyArgs">Args Array</param>
        private static void paramActivitiesInRange(ref string[] legacyArgs)
        {
            //
            // State Param ==> add state filter
            //

            string? stateValue = Config.GetAttività_da_leggereRCommand(); //==> attività_da_leggere could be null

            // If stateValue is set
            if (!String.IsNullOrEmpty(stateValue))
            {
                // Resize Array ==> need to add two element
                Array.Resize(ref legacyArgs, legacyArgs.Length + 2);

                //Add to array
                legacyArgs[legacyArgs.Length-2] = "-state"; //==> state param 
                legacyArgs[legacyArgs.Length-1] = stateValue; //==> state param value
            }
        }
    }
}
