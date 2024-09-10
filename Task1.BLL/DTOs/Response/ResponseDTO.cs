using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.Response
{
    public class ResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class ResponseDTO<T> : ResponseDTO
    {
        public T Data { get; set; }

        public int? Total { get; set; }
    }
}
