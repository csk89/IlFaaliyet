using System;
using System.Collections.ObjectModel;
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
using Action = Turkok.Model.Action;

namespace Gvm.Controllers
{
    [ClaimsAuthorize(Resources.TargetActions.View, Resources.Targets)]
    public class TargetController : MController
    {
        private readonly IRepository<Target> _repository;
        private readonly IRepository<Action> _actionRepository;

        public TargetController(IRepository<Target> repository, IRepository<Action> actionRepository)
        {
            _repository = repository;
            _actionRepository = actionRepository;
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
                Target createdItem = _repository.Add(model);
                return RedirectToAction("Index");
                //return RedirectToAction("View", new { id = createdItem.Id });
            }

            return View(model);
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

        [Audit]
        [HttpPost]
        public ActionResult AddAction(int parentId, Turkok.Model.Action model)
        {
            if (ModelState.IsValid)
            {
                var target = _repository.Find(parentId);
                if(target.Actions == null)
                    target.Actions = new Collection<Action>();
                target.Actions.Add(model);
                _repository.Update(target);
                return RedirectToAction("View", new { id = parentId });
            }
            ViewBag.ParentId = parentId;
            return RedirectToAction("Add", "Action", new {parentId});
        }

        [Audit]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Target target = _repository.Find(id.Value);

            if (target == null)
            {
                return HttpNotFound();
            }

            return View(target);
        }

        [Audit]
        [HttpPost]
        public ActionResult Edit(Target target)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(target);

                return RedirectToAction("View", new { id = target.Id });
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

            Target target = _repository.Table().Include(m => m.Actions).FirstOrDefault(m => m.Id == id.Value);

            if (target == null)
            {
                return HttpNotFound();
            }

            var actions = target.Actions.ToList();

            for(int i=0; i<actions.Count; i++)
            {
                _actionRepository.Remove(actions[i]);
                target.Actions.Remove(actions[i]);
            }

            _repository.Remove(target);

            return RedirectToAction("Index");
        }
    }
}