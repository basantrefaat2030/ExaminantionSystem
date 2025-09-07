namespace ExaminantionSystem.Entities.ViewModels
{
    public class ResponseViewModel<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public ErrorResponseViewModel Error { get; set; }
        public string Message { get; set; }


        public static ResponseViewModel<T> Success(T data, string message = null)
        {
            return new ResponseViewModel<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ResponseViewModel<T> Fail(ErrorResponseViewModel error, string message = null)
        {
            return new ResponseViewModel<T>
            {
                Succeeded = false,
                Error = error,
                Message = message
            };
        }


    }


    public class ErrorResponseViewModel
    {
        public string Type { get; set; }
        public int StatusCode { get; set; }
        public IEnumerable<ErrorDetailViewModel> Errors { get; set; }
        public string URIError { get; set; }
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

