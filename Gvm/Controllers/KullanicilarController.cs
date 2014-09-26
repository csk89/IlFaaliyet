using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Gvm.Infra;
using Gvm.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Core.Mappers;
using Turkok.Core.Repository;
using Turkok.Core.Service;
using Turkok.Model;

namespace Gvm.Controllers
{
    public class KullanicilarController : Controller
    {
        private readonly IUserService _userService;
        private UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Sehir> _cityRepository;
        private readonly IMapper<ApplicationUser, ApplicationUser> _userMapper;

        public KullanicilarController(IUserService userService,
                                    UserManager<ApplicationUser> userManager,
                                    IRepository<Sehir> cityRepository,
                                    IMapper<ApplicationUser, ApplicationUser> userMapper)
        {
            _userService = userService;
            _userManager = userManager;
            _cityRepository = cityRepository;
            _userMapper = userMapper;
        }

        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var kullanicilar = _userService.GetTable().Where(x => x.UserName.ToLower() != "admin@turkok.com");

            return Json(kullanicilar.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveUser(string username)
        {
            if (string.IsNullOrEmpty(username)) return HttpNotFound();

            var user = _userService.GetTable().FirstOrDefault(x => x.UserName == username);
            if (user == null) return HttpNotFound();

            var model = LoadUserModel(username);
            model.Roller = PopulateUserRoles();

            return View(model);
        }

        public ActionResult EditUser(string username)
        {
            if (string.IsNullOrEmpty(username)) return HttpNotFound();

            var user = _userService.GetTable().FirstOrDefault(x => x.UserName == username);
            if (user == null) return HttpNotFound();

            var model = LoadUserModel(username);
            model.Roller = PopulateUserRoles();
            //var userRoleId = string.IsNullOrEmpty(user.UserRole)
            //    ? ""
            //    : roller.Where(x => x.Text == user.UserRole).Select(y => y.Value).FirstOrDefault();

            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveUser(ApproveOrEditUserViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId)) return HttpNotFound();

            if (string.IsNullOrEmpty(model.SelectedRole))
            {
                ModelState.AddModelError("", "Kullanıcıyı aktif yapmak için bir rol seçmelisiniz.");

                model = LoadUserModel(model.UserName);
                model.Roller = PopulateUserRoles();

                return View(model);
            }

            _userManager.RemoveFromRoles(model.UserId, GetAllRoles());
            _userManager.AddToRole(model.UserId, model.SelectedRole);

            var user = _userService.Find(model.UserId);
            user.UserRole = model.SelectedRole;
            user.Active = true;

            _userService.Update(user);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApproveOrEditUserViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId)) return HttpNotFound();

            _userManager.RemoveFromRoles(model.UserId, GetAllRoles());
            var user = _userService.Find(model.UserId);

            if (string.IsNullOrEmpty(model.SelectedRole))
            {
                user.UserRole = null;
                user.Active = false;
            }
            else
            {
                _userManager.AddToRole(model.UserId, model.SelectedRole);

                user.UserRole = model.SelectedRole;
                user.Active = true;
            }
            
            _userService.Update(user);

            return RedirectToAction("Index");
        }

        #region Helpers
        private ApproveOrEditUserViewModel LoadUserModel(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;

            var user = _userService.GetTable().FirstOrDefault(x => x.UserName == username);

            if (user == null) return null;

            return new ApproveOrEditUserViewModel
            {
                UserId = user.Id,
                Active = user.Active,
                UserName = user.UserName,
                AdiSoyadi = user.FullName,
                BasvuruTarihi = user.DateCreated,
                SelectedRole = user.UserRole,
                NotWorkingForUnit = user.NotWorkingForUnit
            };
        }
        private IEnumerable<SelectListItem> PopulateUserRoles()
        {
            var roles = GetAllRoles();
            
            return roles.Select(role => new SelectListItem { Text = role, Value = role }).ToList();
        }
        private string[] GetAllRoles()
        {
            using (var context = new DataContext())
            {
                return context.Roles.OrderBy(x => x.Name).Select(y => y.Name).ToArray();
            }
        }
        #endregion
    }
}