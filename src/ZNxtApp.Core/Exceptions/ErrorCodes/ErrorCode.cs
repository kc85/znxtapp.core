namespace ZNxtApp.Core.Exceptions.ErrorCodes
{
    public static class ErrorCode
    {
        public enum DB
        {
            DUPLICATE_ID = 4000,
            MULTIPLE_ROW_RETURNED = 4010,
            UPDATE_DATA_COUNT_NOT_MATCH = 4020,
            DB_WRITE_ERROR = 4030
        }

        public enum ClientValidation
        {
            CLIENT_REQUIRED_PARAMETER_MISSING = 5000,
            CLIENT_ALREADY_EXISTS = 5010
        }

        public enum ClientCreate
        {
            UNABLE_TO_CREATE_CLIENT = 5500,
            CLIENT_DATA_UNAVALIABLE = 5510
        }

        public enum AuthToken
        {
            AUTH_REQUEST_REQUIRED_PARAMETER_MISSING = 6000
        }

        public enum DBSchemaValidation
        {
            DB_SCHEMA_VALIDATION_ERROR = 6500
        }
    }
}