using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public AccountController(IMailAccountService mailAccountService)
        {
            MailAccountService = mailAccountService;
        }

        public IMailAccountService MailAccountService { get; }

        [HttpPost]
        [Route("MailAuth/Register")]
        public async Task<IActionResult> Register([FromBody] MailRegisterForm mailRegisterForm)
        {
            var result = await MailAccountService.RegisterByMailAndPassword(mailRegisterForm.MailAddress, mailRegisterForm.Password);
            if (result.State == ServiceResultEnum.Success)
            {
                return Json(Success(result.Detail));
            }
            else
            {
                return Json(Result(result.State, result.Detail));
            }
        }

        [HttpGet]
        [Route("MailAuth/Confirm/{token}")]
        public async Task<IActionResult> ConfirmRegister(string token)
        {
            var result = await MailAccountService.AuthMailAddress(token);
            return View(nameof(ServiceMessage), result);
        }

        [HttpPost]
        [Route("MailAuth/Login")]
        public async Task<IActionResult> Login([FromBody] MailLoginForm mailLoginForm)
        {
            var result = await MailAccountService.VerifyMailPassword(mailLoginForm.MailAddress, mailLoginForm.Password);
            return Json(result);
        }

        [HttpGet]
        [Route("MailAuth/ResetPassword/{token}")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            var result = await MailAccountService.ResetPasswordRequestExist(token);
            if (result.ExtraData == null)
            {
                return View(ServiceResultViewName, ServiceMessage.NotFound("您要查看的页面不存在"));
            }
            if (result.State == ServiceResultEnum.TimedOut)
            {
                return View(ServiceResultViewName, TimedOut("该链接已失效"));
            }
            return View(new ResetPasswordForm { EmailAddress = result.ExtraData.EmailAddress });
        }

        [HttpPost]
        [Route("MailAuth/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordForm form)
        {
            return Json(await MailAccountService.RequireResetPasswordByMail(form.MailAddress));
        }

        [HttpPost]
        [Route("MailAuth/ResetPassword/{token}")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordForm form)
        {
            if (!MyForm.ReflectCheck(form))
            {
                return View(ServiceResultViewName, Invalid("参数非法"));
            }
            if (form.Password != form.ConfirmPassword)
            {
                return View(form);
            }
            var result = await MailAccountService.ResetPasswordWithResetTokenAndPassword(form.Token, form.Password);
            if (result.State != ServiceResultEnum.Success)
            {
                return View(ServiceResultViewName, Fail(result.Detail));
            }
            return View(ServiceResultViewName, Success());
        }


    }
}