using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Thinktecture.IdentityModel.Authorization.Mvc;
using Turkok.Core.Repository;
using Turkok.Model.Lookup;
using Action = Turkok.Model.Action;
using Unit = Turkok.Model.Unit;

namespace Gvm.Controllers
{
    [ClaimsAuthorize(Resources.ActionActions.View, Resources.Actions)]
    public class ActionController : MController
    {
        private readonly IRepository<Action> _repository;
        private readonly IRepository<Unit> _unitRepository; 

        public ActionController(IRepository<Action> repository, IRepository<Unit> unitRepository)
        {
            _repository = repository;
            _unitRepository = unitRepository;
        }

        [Audit]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            IOrderedQueryable<Action> actions = _repository.Table().OrderByDescending(x => x.StartDate);

            return Json(actions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            PopulateUnitsDropDownList();
            return View();
        }

        [Audit]
        [HttpPost]
        public ActionResult Create(Action model)
        {
            if (ModelState.IsValid)
            {
                Action createdItem = _repository.Add(model);
                return RedirectToAction("Index");
                //return RedirectToAction("View", new { id = createdItem.Id });
            }

            return View(model);
        }
        private void PopulateUnitsDropDownList(object selectedUnit = null)
        {
            var unitQuery = _unitRepository.Table().OrderBy(u => u.Name);
            ViewBag.DeptChargedId = new SelectList(unitQuery, "Id", "Name", selectedUnit);
        }

        public ActionResult Add(int parentId)
        {
            PopulateUnitsDropDownList();
            ViewBag.ParentId = parentId;
            return View();
        }

        [Audit]
        public ActionResult View(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Action action = _repository.Table().FirstOrDefault(m => m.Id == id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }

            return View(action);
        }

        [Audit]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Action action = _repository.Find(id.Value);

            if (action == null)
            {
                return HttpNotFound();
            }

            PopulateUnitsDropDownList(action.DeptChargedId);
            return View(action);
        }

        [Audit]
        [HttpPost]
        public ActionResult Edit(Action model)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(model);

                return RedirectToAction("View", new { id = model.Id });
            }

            return View();
        }


        [Audit]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Action action = _repository.Table().FirstOrDefault(m => m.Id == id.Value);

            if (action == null)
            {
                return HttpNotFound();
            }

            _repository.Remove(action);

            return RedirectToAction("Index");
        }

    }
}