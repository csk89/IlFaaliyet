using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gvm.Infra;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Serilog;
using Thinktecture.IdentityModel.Authorization.Mvc;
using Turkok.Core;
using Turkok.Core.Extensions;
using Turkok.Core.Repository;
using Turkok.Core.Service;
using Turkok.Model;
using Turkok.Model.Lookup;

namespace Gvm.Controllers
{
    [ClaimsAuthorize(Resources.GonulluVericiMerkeziActions.View, Resources.GonulluVericiMerkezi)]
    public class GonulluVericiMerkeziController : MController
    {
        private readonly IUserService _userService;
        private readonly IRepository<GonulluVericiMerkezi> _repository;
        private readonly IRepository<Sehir> _cityRepository;
        private readonly ILogger _logger;

        public GonulluVericiMerkeziController(IUserService userService, IRepository<GonulluVericiMerkezi> repository, IRepository<Sehir> cityRepository, ILogger logger)
        {
            _userService = userService;
            _repository = repository;
            _cityRepository = cityRepository;
            _logger = logger;
        }


        [Audit]
        public ActionResult Index(int? page)
        {
            ViewBag.Sehirler = _cityRepository.Table().OrderBy(x => x.Adi).ToList();

            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            //var dealerContracts = DealerContact.Join(Dealer,
            //                     contact => contact.DealerId,
            //                     dealer => dealer.DealerId,
            //                     (contact, dealer) => contact);


            //var merkezler = _repository.Table().Where(x => x.IsDeleted == false).ToList();

            var merkezler = _repository.Table().Where(x => x.IsDeleted == false)
                                       .Join(_cityRepository.Table(),
                                            merkez => merkez.SehirId,
                                            city => city.Id,
                                            (merkez, city) => merkez);
            
            // _repository.Table().Join()

            //TODO : @serefbilge join would be more performant
            //TODO : @atagun farklı repository'leri nasıl join edebilirim ? Hata verdi.
            //TODO : yukardaki gibi
            //TODO : tamam, join çalışıyor; peki merkez.SehirAdi = city.SehirAdi eşitliğini nasıl katacağım işe ?

            //foreach (var merkez in merkezler)
            //{
            //    merkez.SehirAdi = _cityRepository.Find(merkez.SehirId).Adi;
            //}

            return Json(merkezler.ToDataSourceResult(request));
        }

        [Audit]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, GonulluVericiMerkezi gonulluVericiMerkezi)
        {
            if (gonulluVericiMerkezi != null && ValidateModel(gonulluVericiMerkezi))
            {
                gonulluVericiMerkezi = _repository.Add(gonulluVericiMerkezi);
                gonulluVericiMerkezi.SehirAdi = _cityRepository.Find(gonulluVericiMerkezi.SehirId).Adi;
            }

            return Json(new[] { gonulluVericiMerkezi }.ToDataSourceResult(request, ModelState));
        }

        [Audit]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, GonulluVericiMerkezi gonulluVericiMerkezi)
        {
            if (gonulluVericiMerkezi != null && ValidateModel(gonulluVericiMerkezi))
            {
                _repository.Update(gonulluVericiMerkezi);
            }

            return Json(new[] { gonulluVericiMerkezi }.ToDataSourceResult(request, ModelState));
        }

        [Audit]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, GonulluVericiMerkezi gonulluVericiMerkezi)
        {
            if (gonulluVericiMerkezi != null && ValidateDelete(gonulluVericiMerkezi))
            {
                gonulluVericiMerkezi.IsDeleted = true;

                _repository.Update(gonulluVericiMerkezi);
            }

            return Json(new[] { gonulluVericiMerkezi }.ToDataSourceResult(request, ModelState));
        }

        #region Validations
        private bool ValidateModel(GonulluVericiMerkezi merkez)
        {
            if (merkez.SehirId == 0)
            {
                ModelState.AddModelError("", "Merkez için bir şehir seçmelisiniz!");
            }

            return ModelState.IsValid;
        }

        private bool ValidateDelete(GonulluVericiMerkezi merkez)
        {
            var userExists = _userService.GetTable().Any(x => x.UnitId == merkez.Id);

            if (userExists)
            {
                ModelState.AddModelError("", "Birime bağlı kullanıcılar olduğundan, birimi kaldıramazsınız");
            }

            return ModelState.IsValid;
        }
        #endregion
    }
}