using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Thinktecture.IdentityModel.Authorization.Mvc;
using Turkok.Core.Repository;
using Turkok.Model;
using Turkok.Model.Lookup;
using Action = Turkok.Model.Action;

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

            return Json(actions.ToDataSourceResult(request));
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
    }
}