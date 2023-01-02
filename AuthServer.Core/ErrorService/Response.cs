using AuthServer.Core.Dtos;
using AuthServer.Core.ErrorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Core.Repository
{
    public class Response<T> where T : class
    {
        public T Data { get; set;}
        public int StatusCode { get; set; }

        [JsonIgnore]
        public bool IsSuccess { get; set; }
        public ErrorDto Error { get; set; }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode , IsSuccess = true };
        }
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccess = true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccess = false

            };
        }

        public static Response<T> Fail(string eroorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(eroorMessage, isShow);
            return new Response<T> { Error=errorDto, StatusCode = statusCode, IsSuccess = false };
        }

    }
}
