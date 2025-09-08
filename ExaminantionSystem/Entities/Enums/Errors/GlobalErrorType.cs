namespace ExaminantionSystem.Entities.Enums.Errors
{
    public enum GlobalErrorType
    {
        Validation = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        BusinessRule = 422,
        Critical = 500
    }
}
