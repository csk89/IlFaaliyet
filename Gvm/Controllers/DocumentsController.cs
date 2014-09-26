using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Turkok.Core.Caching;
using Turkok.Core.Extensions;
using Turkok.Core.Repository;
using Turkok.Core.Service;
using Turkok.Core.Settings;
using Turkok.Model;
using Turkok.Model.FineUploader;

namespace Gvm.Controllers
{
    public class DocumentsController : MController
    {
        private readonly ICacheManager<int, Document> _cache;
        private readonly IRepository<Document> _repository;
        private readonly IApplicationSettings _settings;
        private readonly IUserService _userService;

        public DocumentsController(
            IUserService userService,
            IApplicationSettings appSettings,
            IRepository<Document> repository,
            ICacheManager<int, Document> cache)
        {
            _repository = repository;
            _cache = cache;
            _settings = appSettings;
            _userService = userService;
        }

        [Audit]
        public ActionResult Index(int? page)
        {
            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            IOrderedQueryable<Document> documents = _repository.Table().Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id);

            return Json(documents.ToDataSourceResult(request));
        }

        [Audit]
        public ActionResult View(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Document presentation = _cache.Get(id.Value);

            if (presentation == null)
            {
                presentation = _repository.Table().Include(a => a.Attachments).FirstOrDefault(i => i.Id == id.Value);

                if (presentation == null)
                {
                    return HttpNotFound();
                }

                _cache.Set(id.Value, presentation);
            } 
            return View(presentation);
        }

        [Audit]
        public ActionResult Create()
        {
            return View();
        }
          
        [Audit]
        [HttpPost]
        public ActionResult Create(Document document)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetCurrentUser(User.Identity.GetUserName());

                document.CreateBy = user;
                Document createdItem = _repository.Add(document);

                return RedirectToAction("View", new {id = createdItem.Id});
            }

            return View(document);
        }

        [Audit] 
        public ActionResult Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Document document = _repository.Find(id.Value);

            if (document == null)
            {
                return HttpNotFound();
            }

            _cache.InvalidateCacheItem(id.Value);

            return View(document);
        }

        [Audit]
        [HttpPost]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetCurrentUser(User.Identity.GetUserName());

                document.EditedBy = user;

                _repository.Update(document);

                _cache.InvalidateCacheItem(document.Id);

                return RedirectToAction("View", new {id = document.Id});
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

            _cache.InvalidateCacheItem(id.Value);

            Document document = _repository.Table().FirstOrDefault(m => m.Id == id.Value);

            if (document == null)
            {
                return HttpNotFound();
            }
            var user = _userService.GetCurrentUser(User.Identity.GetUserName());

            document.DeletedBy = user;

            document.IsDeleted = true;

            _repository.Update(document);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public FineUploaderResult UploadToPresentations(FineUpload upload, int? id)
        {
            if (id.HasValue == false)
            {
                return new FineUploaderResult(false, error: "id is null");
            }

            _cache.InvalidateCacheItem(id.Value);

            string guidFilename = AttachmentHelper.CreateGuidFilename(upload.Filename);

            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, _settings.DocumentsServerPath);

            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, _settings.DocumentsAccessPath);
            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment {Name = upload.Filename, Path = accessPath};

                Document document = _repository.Table().Include(x => x.Attachments).SingleOrDefault(x => x.Id == id.Value);

                if (document == null)
                {
                    throw new Exception("Announcement is null");
                }

                if (document.Attachments == null)
                {
                    document.Attachments = new Collection<Attachment>();
                }

                document.Attachments.Add(attachment);

                _repository.Update(document);

                string attachmentsHtml = RenderPartialViewToString("AttachmentList", document.Attachments);

                // the anonymous object in the result below will be convert to json and set back to the browser
                return new FineUploaderResult(true, new {extraInformation = 12345, attachmentsHtml, accessPath});
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}