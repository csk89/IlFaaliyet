using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Turkok.Core.Repository;
using Action = Turkok.Model.Action;

namespace Gvm.Controllers
{
    public class ActionController : MController
    {
        private readonly IRepository<Action> _repository;

        public ActionController(IRepository<Action> repository)
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
            IOrderedQueryable<Action> actions = _repository.Table().OrderByDescending(x => x.StartDate);

            return Json(actions.ToDataSourceResult(request));
        }
        public ActionResult Create()
        {
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


        /*
        [Audit]
        [HttpPost]
        public ActionResult Create(Action action)
        {
            if (ModelState.IsValid)
            {
                Action createdItem = _repository.Add(action);
                return RedirectToAction("Index");
                //return RedirectToAction("View", new { id = createdItem.Id });
            }

            return View(action);
        }
         */
    }
}