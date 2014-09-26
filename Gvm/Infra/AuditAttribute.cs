using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Turkok.Core.Repository;
using Turkok.Model.Audit;

namespace Gvm.Infra
{
    public class AuditAttribute : ActionFilterAttribute
    {
        public IRepository<Audit> Repository { set; get; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            var request = filterContext.HttpContext.Request;

            string sessionIdentifier = string.Empty;

            if (filterContext.HttpContext.Session != null)
            {
                sessionIdentifier = filterContext.HttpContext.Session.SessionID; 
            } 

            var audit = new Audit()
            {
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                UrlAccessed = request.RawUrl,
                Timestamp = DateTime.UtcNow,
                SessionId = sessionIdentifier,
                Data = SerializeRequest(request)
            };

            Repository.Add(audit);

            base.OnActionExecuting(filterContext);
        } 
         
        private string SerializeRequest(HttpRequestBase request)
        {
            return Json.Encode(new { request = request.ToRaw() });
        }
    }
}