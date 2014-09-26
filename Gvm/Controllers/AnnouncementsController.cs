using System; 
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net; 
using System.Web.Mvc;
using Gvm.Infra;
using Gvm.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity; 
using Turkok.Core.Caching;
using Turkok.Core.Extensions;
using Turkok.Core.Mappers;
using Turkok.Core.Repository;
using Turkok.Core.Service; 
using Turkok.Core.Settings;
using Turkok.Model;
using Turkok.Model.FineUploader;

namespace Gvm.Controllers
{
    [Authorize]
    public class AnnouncementsController : MController
    { 
        private readonly IRepository<Announcement> _repository; 
        private readonly IApplicationSettings _settings;
        private readonly ICacheManager<int, Announcement> _cache;
        private readonly IUserService _userService;

        public AnnouncementsController(IRepository<Announcement> repository, 
            IApplicationSettings settings,
            ICacheManager<int, Announcement> cache,
            IUserService userService) 
        {
            _repository = repository; 
            _userService = userService;
            _settings = settings;
            _cache = cache;
        }

        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var announcements = _repository.Table().Where(x => x.IsDeleted == false).OrderByDescending(x=>x.Id);

            return Json(announcements.ToDataSourceResult(request));
        }


        [Audit]
        public ActionResult Create()
        {   
            return View();
        }
         
        [HttpPost]
        [Audit]
        public ActionResult Create(Announcement announcement)
        {
            if (ModelState.IsValid)
            { 

                var user = _userService.GetCurrentUser(User.Identity.GetUserName());

                announcement.CreatedBy = user;

                Announcement instance = _repository.Add(announcement);

                return RedirectToAction("View", new { id = instance.Id });
            }

            return View(announcement);
        }

        [Audit]
        public ActionResult View(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Announcement announcement = _cache.Get(id.Value);

            if (announcement == null)
            { 
                announcement = _repository.Table().Include(a => a.Attachments).Include(u => u.CreatedBy).FirstOrDefault(m => m.Id == id.Value);

                if (announcement == null)
                {
                    return HttpNotFound();
                }

                _cache.Set(id.Value, announcement);
            }

            return View(announcement);
        }

        [Audit] 
        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _cache.InvalidateCacheItem(id.Value);

            Announcement announcement = _repository.Table().FirstOrDefault(u => u.Id == id.Value);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            var user = _userService.GetCurrentUser(User.Identity.GetUserName());

            announcement.DeletedBy = user;

            announcement.IsDeleted = true;
            _repository.Update(announcement);

            return RedirectToAction("Index");
        }

        [Audit] 
        public ActionResult Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _cache.InvalidateCacheItem(id.Value);

            Announcement announcement = _repository.Table().Include(m => m.CreatedBy).FirstOrDefault(u => u.Id == id.Value);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            return View(announcement);
        }

        [Audit] 
        [HttpPost]
        public ActionResult Edit(Announcement announcement)
        {
            if (ModelState.IsValid)
            {  
                var user = _userService.GetCurrentUser(User.Identity.GetUserName());
                announcement.EditedBy = user;

                announcement.DateEdited = DateTime.Now;

                Announcement instance = _repository.Update(announcement);

                return RedirectToAction("View", new { id = instance.Id });
            }

            return View(announcement);
        }
         
        [Audit]
        [HttpPost]
        public FineUploaderResult UploadToAnnouncement(FineUpload upload, int? id)
        {

            if (id.HasValue == false)
            {
                return new FineUploaderResult(false, error: "id is null");
            }

            _cache.InvalidateCacheItem(id.Value);

            var guidFilename = AttachmentHelper.CreateGuidFilename(upload.Filename);

            var uploadPath = AttachmentHelper.GetUploadPath(guidFilename, _settings.AnnouncementServerPath);

            var accessPath = AttachmentHelper.GetAccessPath(guidFilename, _settings.AnnouncementAccessPath);
            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment { Name = upload.Filename, Path = accessPath };

                var announcement = _repository.Table().Include(x => x.Attachments).SingleOrDefault(x => x.Id == id.Value);

                if (announcement == null)
                {
                    throw new Exception("Announcement is null");
                }

                if (announcement.Attachments == null)
                {
                    announcement.Attachments = new Collection<Attachment>();
                }

                announcement.Attachments.Add(attachment);

                _repository.Update(announcement);

                var attachmentsHtml = RenderPartialViewToString("AttachmentList", announcement.Attachments);
                 
                return new FineUploaderResult(true, new { extraInformation = 12345, attachmentsHtml, accessPath });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}