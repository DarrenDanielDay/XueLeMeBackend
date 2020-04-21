using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public enum ServiceResultEnum
    {
        [Display(Name = "操作成功")]
        Success,
        [Display(Name = "操作失败")]
        Fail,
        [Display(Name = "已存在")]
        Exist,
        [Display(Name = "未找到")]
        NotFound,
        [Display(Name = "参数非法")]
        Invalid,
        [Display(Name = "参数合法")]
        Valid,
        [Display(Name = "授权通过")]
        Authorized,
        [Display(Name = "未授权")]
        Unauthorized,
        [Display(Name = "超时失效")]
        TimedOut
    }
    public class ServiceMessage
    {
        [DisplayName("状态")]
        public ServiceResultEnum State { get; set; }
        [DisplayName("详细信息")]
        public string Detail { get; set; }
        public static readonly string ServiceResultViewName = nameof(ServiceMessage);
        public static readonly object EmptyObject = new object { };

        public static ServiceResult<TResult> Result<TResult>(ServiceResultEnum state, TResult data, string detail = null)
        {
            return new ServiceResult<TResult>
            {
                State = state,
                Detail = detail,
                ExtraData = data
            };
        }

        public static ServiceResult<TResult> Success<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Success, data, detail);
        }
        public static ServiceResult<TResult> Fail<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Fail, data, detail);
        }
        public static ServiceResult<TResult> Exist<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Exist, data, detail);
        }
        public static ServiceResult<TResult> NotFound<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.NotFound, data, detail);
        }
        public static ServiceResult<TResult> Invalid<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Invalid, data, detail);
        }
        public static ServiceResult<TResult> Valid<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Valid, data, detail);
        }
        public static ServiceResult<TResult> Authorized<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Authorized, data, detail);
        }
        public static ServiceResult<TResult> Unauthorized<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.Unauthorized, data, detail);
        }

        public static ServiceResult<TResult> TimedOut<TResult>(TResult data, string detail = null)
        {
            return Result(ServiceResultEnum.TimedOut, data, detail);
        }

        public static ServiceResult<object> Result(ServiceResultEnum state, string detail = null)
        {
            return Result<object>(state, null, detail);
        }

        public static ServiceResult<object> Success(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Success, null, detail);
        }
        public static ServiceResult<object> Fail(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Fail, null, detail);
        }
        public static ServiceResult<object> Exist(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Exist, null, detail);
        }
        public static ServiceResult<object> NotFound(string detail = null)
        {
            return Result<object>(ServiceResultEnum.NotFound, null, detail);
        }
        public static ServiceResult<object> Invalid(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Invalid, null, detail);
        }
        public static ServiceResult<object> Valid(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Valid, null, detail);
        }
        public static ServiceResult<object> Authorized(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Authorized, null, detail);
        }
        public static ServiceResult<object> Unauthorized(string detail = null)
        {
            return Result<object>(ServiceResultEnum.Unauthorized, null, detail);
        }
        public static ServiceResult<object> TimedOut(string detail = null)
        {
            return Result<object>(ServiceResultEnum.TimedOut, null, detail);
        }
    }

    public class ServiceResult<TResult> : ServiceMessage
    {
        public TResult ExtraData { get; set; }
    }
}
