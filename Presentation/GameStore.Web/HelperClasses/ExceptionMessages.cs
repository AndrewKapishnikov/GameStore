using Microsoft.AspNetCore.Http;

namespace GameStore.Web.HelperClasses
{
    static class ExceptionMessages
    {
        public const string NoElements = "Sequence contains no elements.";
        public const string EmptySession = "Session is empty";
        public const string DatabaseNoConnectionString = "Value cannot be null. (Parameter 'connectionString')";
        public const string UpdatingEntriesError = "An error occurred while updating the entries. See the inner exception for details.";
        public const string NoEstablishConnectionToSQLServer = "A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: SNI_PN11, error: 26 - Error Locating Server/Instance Specified)";
    }
}
