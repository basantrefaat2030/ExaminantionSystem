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
        public string Code { get; set; } 
        public string Title { get; set; } 
        public string Detail { get; set; } 
        public string Source { get; set; } // Field/parameter causing error
        //public Dictionary<string, object> Meta { get; set; } // Additional context

        public ErrorDetail(string code , string title, string detail = null,
                          string source = null)
        {
            Code = code;
            Title = title;
            Detail = detail;
            Source = source;
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
