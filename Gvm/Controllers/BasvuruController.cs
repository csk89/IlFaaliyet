using System; 
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq; 
using System.Web.Mvc;
using Gvm.Infra;
using Gvm.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI; 
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using Turkok.Core.Mappers;
using Turkok.Core.Repository;
using Turkok.Model;
using Turkok.Core.Service;

namespace Gvm.Controllers
{
    [Authorize]
    public class BasvuruController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRepository<Basvuru> _repository;
        private readonly IRepository<GonulluVerici> _vericiRepository;
        private readonly IRepository<GonulluVericiMerkezi> _merkezlerRepository;
        private readonly IMapper<BasvuruViewModel, Basvuru> _basvuruMapper;
        private readonly IRepository<KaraListeGonulluVerici> _karalisteRepository; 


        public BasvuruController(
            IUserService userService,
            IRepository<KaraListeGonulluVerici> karalisteRepository,
            IRepository<Basvuru> repository,
            IRepository<GonulluVerici> vericiRepository,
            IRepository<GonulluVericiMerkezi> merkezlerRepository,
            IMapper<BasvuruViewModel, Basvuru> basvuruMapper)
        {
            _karalisteRepository = karalisteRepository;
            _userService = userService;
            _repository = repository;
            _vericiRepository = vericiRepository;
            _merkezlerRepository = merkezlerRepository;
            _basvuruMapper = basvuruMapper;
        }

        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        [Audit]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var basvurular = _repository.Table().Include(g => g.GonulluVerici).Where(x => x.IsDeleted == false && x.GonulluVerici.IsDeleted == false);

            return Json(basvurular.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BifCiktisi(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            var basvuru = _repository.Table().FirstOrDefault(x => x.Id == id.Value && x.IsDeleted == false);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.IsDeleted)
            {
                // another page?
                return HttpNotFound();
            }
             
            basvuru.BifCiktisiTamamlandi = true;

            _repository.Update(basvuru);
            return RedirectToAction("Details", new { id = id.Value });
        }

        [Audit]
        public ActionResult BasvuruyuPasifYap(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            var basvuru = _repository.Table().FirstOrDefault(x => x.Id == id.Value && x.IsDeleted == false);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.IsDeleted)
            {
                // another page?
                return HttpNotFound();
            }

            basvuru.Active = false;
            _repository.Update(basvuru);

            return RedirectToAction("Details", new { id = id.Value });
        }

        [Audit]
        public ActionResult BasvuruyuAktifYap(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }
            var basvuru = _repository.Table().FirstOrDefault(x => x.Id == id.Value && x.IsDeleted == false);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.IsDeleted)
            {
                // another page?
                return HttpNotFound();
            }

            basvuru.Active = true;
            _repository.Update(basvuru);

            return RedirectToAction("Details", new { id = id.Value });
        }

        [Audit]
        public ActionResult BasvuruyuTamamla(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            var basvuru = _repository.Table().Include(g=>g.GonulluVerici).FirstOrDefault(x => x.Id == id.Value && x.IsDeleted == false);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.IsDeleted)
            {
                // another page?
                return HttpNotFound();
            }

            if (basvuru.IsComplete() && basvuru.BifCiktisiTamamlandi)
            {
                basvuru.Completed = true;
                basvuru.Active = true;

                _repository.Update(basvuru);

            }
            else
            {
               //TODO : add error messagaes. 
            }
              
            return RedirectToAction("Details", new { id = id.Value });
        }

        [Audit]
        public ActionResult DeleteBasvuru(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }
            var basvuru = _repository.Table().Include(g=>g.GonulluVerici).FirstOrDefault(x => x.Id == id.Value && x.IsDeleted == false);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.Completed)
            {
                // basvuru tamamlanmissa silinemez.
                return View();
            }

            basvuru.IsDeleted = true;
            basvuru.GonulluVerici.IsDeleted = true;

            _repository.Update(basvuru);


            return RedirectToAction("Index");
        }

        [Audit]
        public ActionResult Details(int? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            var basvuru = _repository.Table().Include(x => x.GonulluVerici).Include(i => i.GonulluVerici.VericiIletisimBilgileri).Include(u => u.CreatedBy).Include(g => g.GonulluVericiMerkezi).FirstOrDefault(x => x.Id == id.Value);

            if (basvuru == null)
            {
                return HttpNotFound();
            }

            if (basvuru.IsDeleted)
            {
                //TODO : we need to handle this
                return HttpNotFound();
            }

            return View(basvuru);
        }
        private IEnumerable<SelectListItem> PopulateGonulluVericiMerkezleriDropdown()
        {
            var items =
                _merkezlerRepository.Table()
                    .OrderBy(x => x.Adi)
                    .ToList()
                    .Select(x => new SelectListItem { Text = x.Adi, Value = x.Id.ToString() });

            return items;
        }

        [Audit]
        public ActionResult CreateBasvuru()
        {
            var viewModel = new BasvuruViewModel
            {
                GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown(),
                GonulluVerici = new GonulluVerici(),
                Active =  true
            };

            return View(viewModel);
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBasvuru(BasvuruViewModel basvuruViewModel)
        {
            if (ModelState.IsValid)
            {

                if (basvuruViewModel.GonulluVerici.Cinsiyet == 0)
                {
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    ModelState.AddModelError("Error", "Cinsiyet seciniz!");
                    return View(basvuruViewModel);
                }

                var gonulluVericiVarmi =
                    _vericiRepository.Table().Any(x => x.TcKimlikNo == basvuruViewModel.GonulluVerici.TcKimlikNo);

                if (gonulluVericiVarmi)
                {
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    ModelState.AddModelError("Error", "Bu TCKimlik ile basvuru yapilmis. Devam edemezsiniz!");
                    return View(basvuruViewModel);
                }

                var karalistedemi = _karalisteRepository.Table().Any(x => x.GonulluVerici.TcKimlikNo == basvuruViewModel.GonulluVerici.TcKimlikNo);

                if (karalistedemi)
                {
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    ModelState.AddModelError("Error","Bu aday kara listede. Basvuruya devam edemezsiniz!");
                    return View(basvuruViewModel);
                }

                var basvuru = _basvuruMapper.ToModel(basvuruViewModel);
                basvuru.CreatedBy = _userService.GetCurrentUserFromDatabase(User.Identity.GetUserName());
                basvuru.Active = true;

                int gonulluVericiMerkeziId;

                if (int.TryParse(basvuruViewModel.SelectedGonulluVericiMerkezi, out gonulluVericiMerkeziId))
                {
                    basvuru.GonulluVericiMerkezi = _merkezlerRepository.Find(gonulluVericiMerkeziId);
                }
                else
                {
                    ModelState.AddModelError("Error", "Gonullu verici Merkezi Seciniz!");
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    return View(basvuruViewModel);
                }
                 
                basvuru.GonulluVerici.GonulluVericiMerkezi = gonulluVericiMerkeziId;

                basvuru = _repository.Add(basvuru);

                return RedirectToAction("Index", new { id = basvuru.Id });

            }

            basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();

            return View(basvuruViewModel);
        }


        public ActionResult EditBasvuru(int basvuru)
        {
            var basvuruinstance = _repository.Table().Include(g=>g.GonulluVerici).Include(v=>v.GonulluVericiMerkezi).FirstOrDefault(x => x.Id == basvuru);

            if (basvuruinstance == null)
            {
                return HttpNotFound();
            }

            var viewModel = new BasvuruViewModel
            {
                GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown(),
                GonulluVerici = basvuruinstance.GonulluVerici,
                SelectedGonulluVericiMerkezi = basvuruinstance.GonulluVericiMerkezi.Id.ToString(),
                Id = basvuru
            };

            return View(viewModel);
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBasvuru(BasvuruViewModel basvuruViewModel)
        {
            if (ModelState.IsValid)
            {
                var basvuru = _repository.Table().Include(g => g.GonulluVerici).Include(m => m.GonulluVericiMerkezi).FirstOrDefault(x => x.Id == basvuruViewModel.Id);

                if (basvuru == null)
                {
                    return HttpNotFound();
                }

                if (basvuruViewModel.GonulluVerici.Cinsiyet == 0)
                {
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    ModelState.AddModelError("Error", "Cinsiyet seciniz!");
                    return View(basvuruViewModel);
                }
                     
                basvuru.EditedBy = _userService.GetCurrentUserFromDatabase(User.Identity.GetUserName());

                basvuru.GonulluVerici = _vericiRepository.Find(basvuruViewModel.GonulluVerici.Id);

                int gonulluVericiMerkeziId;

                if (int.TryParse(basvuruViewModel.SelectedGonulluVericiMerkezi, out gonulluVericiMerkeziId))
                {
                    basvuru.GonulluVericiMerkezi = _merkezlerRepository.Find(gonulluVericiMerkeziId);
                }
                else
                {
                    ModelState.AddModelError("Error", "Gonullu verici Merkezi Seciniz!");
                    basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();
                    return View(basvuruViewModel);
                }

                var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == basvuruViewModel.GonulluVerici.Id);

                if (gonulluVerici == null)
                {
                    return HttpNotFound();
                }

                gonulluVerici.GonulluVericiMerkezi = gonulluVericiMerkeziId;

                gonulluVerici.AdiSoyadi = basvuruViewModel.GonulluVerici.AdiSoyadi;
                gonulluVerici.TcKimlikNo = basvuruViewModel.GonulluVerici.TcKimlikNo;
                gonulluVerici.DogumTarihi = basvuruViewModel.GonulluVerici.DogumTarihi;
                gonulluVerici.Yas = basvuruViewModel.GonulluVerici.Yas;
                gonulluVerici.Cinsiyet = basvuruViewModel.GonulluVerici.Cinsiyet;
                 
                basvuru.GonulluVerici = gonulluVerici;
                 
                basvuru = _repository.Update(basvuru);

                return RedirectToAction("Details", new { id = basvuru.Id });

            }

            basvuruViewModel.GonulluVericiMerkezleri = PopulateGonulluVericiMerkezleriDropdown();

            return View(basvuruViewModel);
        }
         
        [Audit]
        public ActionResult CreateVericiYakiniIletisimBilgileri(int gv)
        {
            var vericiYakini = new GonulluVericiYakini { GonulluVerici = gv, IletisimBilgileri =  new IletisimBilgileri()};

            return View("EditVericiYakiniIletisim", vericiYakini);
        }

        [Audit]
        public ActionResult EditVericiYakiniIletisimBilgileri(int gv)
        {
            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gv);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            return View("EditVericiYakiniIletisim", gonulluVerici.GonulluVericiYakini);
        }

        [Audit]
        [HttpPost]
        public ActionResult EditVericiYakiniIletisimBilgileri(GonulluVericiYakini gonulluVericiYakini)
        {
            if (ModelState.IsValid == false)
            {
                return View("EditVericiYakiniIletisim", gonulluVericiYakini);
            }

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gonulluVericiYakini.GonulluVerici);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            gonulluVerici.GonulluVericiYakini = gonulluVericiYakini;

            gonulluVerici.GonulluVericiYakini.IletisimBilgileri.GonulluVerici = gonulluVericiYakini.GonulluVerici;

            _vericiRepository.Update(gonulluVerici);

            var basvuru = _repository.Table().FirstOrDefault(x => x.GonulluVerici.Id == gonulluVerici.Id);

            if (basvuru == null)
            {
                throw new Exception("Basvuru doesnt exists");
            }

            return RedirectToAction("Details", new { id = basvuru.Id });
        }

        [Audit]
        public ActionResult CreateVericiIletisimBilgileri(int gv)
        {
            var iletisim = new IletisimBilgileri { GonulluVerici = gv };

            return View("EditVericiIletisimBilgileri", iletisim);
        }

        [Audit]
        [HttpPost]
        public ActionResult EditVericiIletisimBilgileri(IletisimBilgileri iletisimBilgileri)
        {
            if (ModelState.IsValid == false)
            {
                return View(iletisimBilgileri);
            }

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == iletisimBilgileri.GonulluVerici);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }
               
            gonulluVerici.VericiIletisimBilgileri = iletisimBilgileri;

            _vericiRepository.Update(gonulluVerici);

            var basvuru = _repository.Table().FirstOrDefault(x => x.GonulluVerici.Id == gonulluVerici.Id);

            if (basvuru == null)
            {
                throw new Exception("Basvuru doesnt exists");
            }

            return RedirectToAction("Details", new { id = basvuru.Id });
        }

        [Audit]
        public ActionResult EditVericiIletisimBilgileri(int gv)
        { 
            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gv);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            return View(gonulluVerici.VericiIletisimBilgileri);
        }

        public ActionResult CreateHastaliklar(int gv)
        {

            var hastaliklar = new Hastaliklar
            {
                GonulluVerici = gv
            };

            return View("EditHastaliklar", hastaliklar);
        }

        public ActionResult EditHastaliklar(int gv)
        {
            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gv);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            return View("EditHastaliklar", gonulluVerici.Hastaliklar);
        }

        [HttpPost]
        public ActionResult EditHastaliklar(Hastaliklar hastaliklar)
        {
            if (ModelState.IsValid == false)
            {
                return View("EditHastaliklar", hastaliklar);
            }

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == hastaliklar.GonulluVerici);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }  
            var basvuru = _repository.Table().FirstOrDefault(x => x.GonulluVerici.Id == gonulluVerici.Id);

            if (basvuru == null)
            {
                throw new Exception("Basvuru doesnt exists");
            }
             
            if (hastaliklar.AidsHiv || hastaliklar.HepatitB || hastaliklar.HepatitC || hastaliklar.Kanser ||
                hastaliklar.Sifiliz || hastaliklar.SurekliTedaviOlmasiGereklimi)
            {

                var karalisteEntity = new KaraListeGonulluVerici {GonulluVerici = gonulluVerici};
                gonulluVerici.KaraListedemi = true;

                var karalistedemi = _karalisteRepository.Table().Any(x => x.GonulluVerici.TcKimlikNo == gonulluVerici.TcKimlikNo);

                if (karalistedemi)
                {
                    _karalisteRepository.Update(karalisteEntity);
                }
                else
                {
                    _karalisteRepository.Add(karalisteEntity);
                }  
            }

            gonulluVerici.Hastaliklar = hastaliklar;
             
            _vericiRepository.Update(gonulluVerici);
             
            return RedirectToAction("Details", new { id = basvuru.Id });

        }

        public ActionResult CreateSerolojikTestler(int gv)
        {

            var seralojikTestler = new SerolojikTestler 
            {
                GonulluVerici = gv
            };

            return View("EditSerolojikTestler", seralojikTestler);
        }

        public ActionResult EditSerolojikTestler(int gv)
        {

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gv);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            return View("EditSerolojikTestler", gonulluVerici.SerolojikTestler);
        }

        [HttpPost]
        public ActionResult EditSerolojikTestler(SerolojikTestler serolojikTestler)
        {
            if (ModelState.IsValid == false)
            {
                return View("EditSerolojikTestler", serolojikTestler);
            }

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == serolojikTestler.GonulluVerici);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }
  
            var basvuru = _repository.Table().FirstOrDefault(x => x.GonulluVerici.Id == gonulluVerici.Id);

            if (basvuru == null)
            {
                throw new Exception("Basvuru doesnt exists");
            }

            if (serolojikTestler.AntiHcv || serolojikTestler.AntiHiv || serolojikTestler.HBsAg || serolojikTestler.HBsAg)
            {
                var karalisteitem = new KaraListeGonulluVerici {GonulluVerici = gonulluVerici};

                _karalisteRepository.Add(karalisteitem);

                gonulluVerici.KaraListedemi = true; 
            }

            gonulluVerici.SerolojikTestler = serolojikTestler;

            _vericiRepository.Update(gonulluVerici);
             
            return RedirectToAction("Details", new { id = basvuru.Id });

        }

        public ActionResult CreateHematolojikTestler(int gv)
        {

            var hematolojikTestler = new HematolojikTestler 
            {
                GonulluVerici = gv
            };

            return View("EditHematolojikTestler", hematolojikTestler);
        }

        public ActionResult EditHematolojikTestler(int gv)
        {

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == gv);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            return View("EditHematolojikTestler", gonulluVerici.HematolojikTestler);
        }

        [HttpPost]
        public ActionResult EditHematolojikTestler(HematolojikTestler hematolojikTestler)
        {
            if (ModelState.IsValid == false)
            {
                return View("EditHematolojikTestler", hematolojikTestler);
            }

            var gonulluVerici = _vericiRepository.Table().FirstOrDefault(x => x.Id == hematolojikTestler.GonulluVerici);

            if (gonulluVerici == null)
            {
                return HttpNotFound();
            }

            gonulluVerici.HematolojikTestler = hematolojikTestler;

            _vericiRepository.Update(gonulluVerici);

            var basvuru = _repository.Table().FirstOrDefault(x => x.GonulluVerici.Id == gonulluVerici.Id);

            if (basvuru == null)
            {
                throw new Exception("Basvuru doesnt exists");
            }

            return RedirectToAction("Details", new { id = basvuru.Id });
        }

        public ActionResult KaraListe()
        {
            return View();
        }

        [Audit]
        public ActionResult ReadKaraListe([DataSourceRequest] DataSourceRequest request)
        {
            var karaliste = _karalisteRepository.Table().Include(x=>x.GonulluVerici).Where(x => x.IsDeleted == false && x.GonulluVerici.IsDeleted == false);

            return Json(karaliste.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BasvuruReport()
        {
            var toplamBasvuru = _repository.Table().Count();

            var aktifactiveBasvuru = _repository.Table().Count(x => x.IsDeleted == false && x.Active);

            var tamamlananBasvuru = _repository.Table().Count(x => x.IsDeleted == false && x.Completed);

            var silinenBasvuru = _repository.Table().Count(x => x.IsDeleted);
            
            var pasifBasvuru = _repository.Table().Count(x => x.IsDeleted && x.Active == false);

            var karaliste = _karalisteRepository.Table().Count();

            return Json(new { toplamBasvuru, aktifactiveBasvuru, tamamlananBasvuru, silinenBasvuru, pasifBasvuru, karaliste }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            return PartialView("_SideBar");
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView();
        }
         
    }
}