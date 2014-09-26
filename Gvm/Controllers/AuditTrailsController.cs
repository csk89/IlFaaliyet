using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Turkok.Core.Repository;
using Turkok.Model.Audit;

namespace Gvm.Controllers
{
    public class AuditTrailsController : Controller
    {
        private readonly IRepository<Audit> _repository;

        public AuditTrailsController(IRepository<Audit> repository)
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
            return Json(_repository.Table().OrderByDescending(x => x.Id).ToDataSourceResult(request));
        }
    }
}