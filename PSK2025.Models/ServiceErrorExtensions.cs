using PSK2025.Models.Enums;

namespace PSK2025.Models.Extensions
{
    public static class ServiceErrorExtensions
    {
        public static string GetErrorMessage(this ServiceError error, string? entityName = null)
        {
            var entity = string.IsNullOrEmpty(entityName) ? "resource" : entityName.ToLowerInvariant();

            return error switch
            {
                ServiceError.NotFound => $"The requested {entity} was not found.",
                ServiceError.Disabled => $"The {entity} functionality is temporarily disabled. Please try again later.",
                ServiceError.AlreadyExists => $"A {entity} with the same identifier already exists.",
                ServiceError.InvalidData => $"The provided {entity} data is invalid.",
                ServiceError.Unauthorized => "You are not authenticated to perform this operation.",
                ServiceError.Forbidden => "You do not have permission to perform this operation.",
                ServiceError.DatabaseError => $"A database error occurred while processing the {entity}.",
                ServiceError.Unknown or _ => "An unexpected error occurred."
            };
        }

        public static int GetStatusCode(this ServiceError error)
        {
            return error switch
            {
                ServiceError.None => 200,
                ServiceError.Disabled => 503,
                ServiceError.NotFound => 404,
                ServiceError.AlreadyExists => 409,
                ServiceError.InvalidData => 400,
                ServiceError.Unauthorized => 401,
                ServiceError.Forbidden => 403,
                ServiceError.DatabaseError => 500,
                ServiceError.Unknown or _ => 500
            };
        }
    }
}