using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Gvm.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Core.Repository;
using Turkok.Core.Service;
using Turkok.Model;

namespace Gvm.Controllers
{
    [Authorize(Roles = "Yönetici")]
    public class ClaimAuthorisationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly IClaimAuthorisationService _service;

        public ClaimAuthorisationController(IUserService userService,
                                            IRepository<IdentityRole> roleRepository, 
                                            IClaimAuthorisationService service)
        {
            _userService = userService;
            _roleRepository = roleRepository;
            _service = service;
        }

        [Audit]
        public ActionResult Index(int? page)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary) TempData["ViewData"];
            }

            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var roller = _roleRepository.Table();

            return Json(roller.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [Audit]
        public ActionResult RoleAuthorisations(string roleName)
        {
            var roleAuthorisations = _service.GetRoleAuthorisations(roleName);

            ViewBag.RoleName = roleName;

            return View(roleAuthorisations);
        }

        [Audit]
        public JsonResult SetActionAuthorisation(string roleName, string resourceName, string actionName, bool toBeChecked)
        {
            if (_service.CheckActionAuthorisationVariables(roleName, resourceName, actionName) == false)
                return Json(new { IsDone = "", Error = "Değişkenler geçersiz" }, JsonRequestBehavior.AllowGet);
            
            var isChecked = _service.IsActionAuthorizedForRole(roleName, resourceName, actionName);

            if (isChecked == toBeChecked)
            {
                var isDone = toBeChecked ? "AlreadyChecked" : "AlreadyUnchecked";

                return Json(new { IsDone = isDone, Error = "" }, JsonRequestBehavior.AllowGet);
            }

            if (toBeChecked && isChecked == false)
            {
                var newClaimAuthorisation = new ClaimAuthorisation
                {
                    Claim = roleName,
                    ClaimType = ClaimTypes.Role,
                    Resource = resourceName,
                    Action = actionName
                };

                _service.Create(newClaimAuthorisation);

                return Json(new { IsDone = "Checked", Error = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var claimAuthorisationInDb = _service.FindRoleClaim(roleName, resourceName, actionName);

                _service.Delete(claimAuthorisationInDb);

                return Json(new { IsDone = "Unchecked", Error = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "Rol adı girmediniz!");

                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }


            using (var context = new DataContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (roleManager.RoleExists(roleName) == false)
                {
                    var role = new IdentityRole { Name = roleName };

                    roleManager.Create(role);
                }
                else
                {
                    ModelState.AddModelError("", "Bu rol zaten mevcuttur.");

                    TempData["ViewData"] = ViewData;
                }
            }

            return RedirectToAction("Index");
        }

        [Audit]
        public ActionResult DeleteRole(string roleName)
        {
            var isInUse = _userService.GetTable().Any(x => x.UserRole == roleName);

            if (isInUse)
            {
                ModelState.AddModelError("", "Bu rol kullanımdadır.");
                TempData["ViewData"] = ViewData;

                return RedirectToAction("Index");
            }

            foreach (var item in _service.GetTable().Where(x => x.Claim == roleName && x.ClaimType == ClaimTypes.Role).ToList())
            {
                _service.Delete(item);
            }
            
            using (var context = new DataContext())
            {
                if (context.Roles.Any(x => x.Name == roleName) == false)
                {
                    ModelState.AddModelError("", "Böyle bir rol mevcut değildir.");
                    TempData["ViewData"] = ViewData;
                }

                var role = context.Roles.FirstOrDefault(x => x.Name == roleName);
                context.Roles.Remove(role);

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}