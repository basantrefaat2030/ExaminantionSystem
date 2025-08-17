namespace ExaminantionSystem.Entities.Wrappers
{
    public class ErrorResponse
    {
        public ErrorType Type { get; set; }
        public string TypeDescription => Type.ToString();
        public int StatusCode => (int)Type;
        public IEnumerable<ErrorDetail> Errors { get; set; }
        public string URIError { get; set; }

        public ErrorResponse(ErrorType type, IEnumerable<ErrorDetail> errors, string UriError = null)
        {
            Type = type;
            Errors = errors;
            URIError = UriError;
        }
    }



    public class ErrorDetail
    {
        public string Code { get; set; } // e.g., "USER_EMAIL_EXISTS"
        public string Title { get; set; } // Short error title
        public string Detail { get; set; } // Detailed explanation
        public string Source { get; set; } // Field/parameter causing error
        public Dictionary<string, object> Meta { get; set; } // Additional context

        public ErrorDetail(string code, string title, string detail = null,
                          string source = null, Dictionary<string, object> meta = null)
        {
            Code = code;
            Title = title;
            Detail = detail;
            Source = source;
            Meta = meta ?? new Dictionary<string, object>();
        }

    }

    public enum ErrorType
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
