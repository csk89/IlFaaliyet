using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Thinktecture.IdentityModel.Authorization.Mvc;
using Turkok.Core.Repository;
using Turkok.Model;
using Turkok.Model.Lookup;

namespace Gvm.Controllers
{
    [ClaimsAuthorize(Resources.TargetActions.View, Resources.Targets)]
    public class TargetController : MController
    {
        private readonly IRepository<Target> _repository;

        public TargetController(IRepository<Target> repository)
        {
            _repository = repository;
        }
        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            IOrderedQueryable<Target> targets = _repository.Table().OrderByDescending(x => x.Id);

            return Json(targets.ToDataSourceResult(request));
        }
        public ActionResult Create()
        {
            return View();
        }

        [Audit]
        [HttpPost]
        public ActionResult Create(Target model)
        {
            if (ModelState.IsValid)
            {

                model.CreationDate = GetLocalTime();

                Target createdItem = _repository.Add(model);
                return RedirectToAction("Index");
                //return RedirectToAction("View", new { id = createdItem.Id });
            }

            return View(model);
        }
        private DateTime GetLocalTime()
        {
            DateTime dateTime = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, tzi);
        }

        [Audit]
        public ActionResult View(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Target target = _repository.Table().Include(a => a.Actions).FirstOrDefault(m => m.Id == id.Value);
            if (target == null)
            {
                return HttpNotFound();
            }
            
            return View(target);
        }
    }
}