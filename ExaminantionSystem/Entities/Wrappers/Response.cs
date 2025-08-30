namespace ExaminantionSystem.Entities.Wrappers
{
    public class Response<T>  
    {
        // Success constructors
        public static Response<T> Success(T data) => new() { Data = data, Succeeded = true  , Message = string.Empty};

        // Error constructors
        public static Response<T> Fail(ErrorType type, params ErrorDetail[] errors)
            => new() { Error = new ErrorResponse(type, errors), Succeeded = false };

        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public ErrorResponse Error { get; set; }
        public string Message { get; set; }
    }

    #region PagedResponseModel
    public class PagedResponse<T> : Response<IEnumerable<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords) 
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }

    #endregion
}
