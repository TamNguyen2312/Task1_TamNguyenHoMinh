using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.Response
{
    public class ResponseApiDTO
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessage {  get; set; }
        public object Result { get; set; }
    }
}
