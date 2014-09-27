using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Gvm.Models;
using Turkok.Model;

namespace Gvm.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Audit]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Şifreniz yenilenmiştir."
                : message == ManageMessageId.ChangeWorkPhoe ? "Ev/İş telefonunuz yenilenmiştir."
                : message == ManageMessageId.ChangeCellPhone ? "Cep telefonunuz yenilenmiştir."
                : message == ManageMessageId.Error ? "Bir hata oluştu."
                : "";

            if (TempData["ViewData"] != null) ViewData = (ViewDataDictionary) TempData["ViewData"];

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var model = new IndexViewModel
                {
                    WorkPhone = user.WorkPhone,
                    CellPhone = user.CellPhone,
                    Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId())
                };

                return View(model);
            }

            return RedirectToAction("Login", "Account");
        }
        
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }
        
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;

                return RedirectToAction("Index");
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);
            TempData["ViewData"] = ViewData;

            return RedirectToAction("Index");
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeWorkPhone(string workphone)
        {
            if (workphone.IsPhoneValid() == false)
            {
                ModelState.AddModelError("", "Telefon numarası başındaki sıfır hariç 10 haneli olmalıdır.");
                TempData["ViewData"] = ViewData;

                return RedirectToAction("Index");
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                user.WorkPhone = workphone;

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangeWorkPhoe });
                }
            }

            return RedirectToAction("Login", "Account");
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeCellPhone(string cellphone)
        {
            if (cellphone.IsPhoneValid() == false)
            {
                ModelState.AddModelError("", "Telefon numarası başındaki sıfır hariç 10 haneli olmalıdır.");
                TempData["ViewData"] = ViewData;

                return RedirectToAction("Index");
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                user.CellPhone = cellphone;

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangeCellPhone });
                }
            }

            return RedirectToAction("Login", "Account");
        }
        
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        [Audit]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            ChangeWorkPhoe,
            ChangeCellPhone,
            ChangePasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        #endregion
    }
}