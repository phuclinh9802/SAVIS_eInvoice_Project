using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{

    // RULES
    // If success, status code = 1, message is empty, data is not null
    // If error, status code = 0, message is error message, data is null

    public class Response2<T> : Response
    {
        public T Data { get; set; }

        public int TotalCount { get; set; }

        public int DataCount { get; set; }
        

        public Response2(int status, string message = null, T data = default(T))
                : base(status, message)
        {
            Data = data;
            TotalCount = 0;
            DataCount = 0;
        }

        public Response2(int status, string message = null, T data = default(T), int dataCount = 0, int totalCount = 0)
            : base(status, message)
        {
            Data = data;
            TotalCount = totalCount;
            DataCount = dataCount;
        }
    }

    // RULES
    // If success, status code = 1, message is empty, data is not null
    // If error, status code = 0, message is error message, data is null
    public class Response<T> : Response
    {
        public T Data { get; private set; }

        public Response(int status, string message = null, T data = default(T))
            : base(status, message)
        {
            Data = data;
        }
    }

    public class Response
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public Response(int status, string message = null)
        {
            Status = status;
            Message = message;
        }
    }
}
