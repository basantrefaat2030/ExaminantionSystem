using ExaminantionSystem.Entities.Wrappers;

namespace ExaminantionSystem.Entities.ViewModels
{
    public class ResponseViewModel<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public ErrorResponseViewModel Error { get; set; }
        public string Message { get; set; }


        public static ResponseViewModel<T> Success(T data) => new() { Data = data, Succeeded = true, Message = string.Empty };

        // Error constructors
        public static ResponseViewModel<T> Fail(ErrorType type, params ErrorDetail[] errors) => new() { Error = new ErrorResponseViewModel(type, errors), Succeeded = false };


    }


    public class ErrorResponseViewModel
    {
        public ErrorType Type { get; set; }
        public int StatusCode => (int)Type;
        public IEnumerable<ErrorDetailViewModel> Errors { get; set; }
        public string URIError { get; set; }

        public ErrorResponseViewModel(ErrorType type, IEnumerable<ErrorDetailViewModel> errors, string UriError = null)
        {
            Type = type;
            Errors = errors;
            URIError = UriError;
        }
    }

    public class ErrorDetailViewModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Source { get; set; }
    }


    #region PagedResponseViewModel
    public class PagedResponseViewModel<T> : ResponseViewModel<IEnumerable<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponseViewModel(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            //(Math.Ceiling) This rounds up to the nearest integer.
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }

    #endregion
}

