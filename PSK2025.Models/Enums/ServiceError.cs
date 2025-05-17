namespace PSK2025.Models.Enums
{
    public enum ServiceError
    {
        None,
        NotFound,
        AlreadyExists,
        InvalidData,
        Unauthorized,
        Forbidden,
        DatabaseError,
        ConcurrencyError,
        Unknown
    }
}