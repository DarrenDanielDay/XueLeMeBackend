using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;

namespace XueLeMeBackend.Services
{
    public interface IMailAccountService
    {
        /// <summary>
        /// See if the mail address is already registered.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public Task<ServiceResult<Authentication>> MailExists(string mail);
        /// <summary>
        /// Register by mail address and password.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<ServiceResult<object>> RegisterByMailAndPassword(string mail, string password);
        /// <summary>
        /// Reset password if the mail address is registered.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public Task<ServiceResult<object>> RequireResetPasswordByMail(string mail);
        /// <summary>
        /// Change password with the given password.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        public Task<ServiceResult<object>> ChangePasswordByMailAndOldPassword(string mail, string oldpassword, string newpassword);
        /// <summary>
        /// Check if the password matches the mail address.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ServiceResult<bool>> VerifyMailPassword(string mail, string token);
        /// <summary>
        /// See if the reset password request exist.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ServiceResult<ResetPasswordRequest>> ResetPasswordRequestExist(string token);
        /// <summary>
        /// Reset the password to the new password given.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        public Task<ServiceResult<object>> ResetPasswordWithResetTokenAndPassword(string token, string newpassword);
        /// <summary>
        /// Authorize the mail address with token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ServiceResult<bool>> AuthMailAddress(string token);
        public Task<ServiceResult<int?>> UserIdOfMail(string mail);
    }
}
