using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Turkok.Core.Repository;
using Turkok.Model;
using Turkok.Model.Lookup;

namespace Turkok.Core.Service
{
    public class ClaimAuthorisationService : IClaimAuthorisationService
    {
        private readonly IRepository<ClaimAuthorisation> _repository;
        private readonly IRepository<IdentityRole> _roleRepository;

        public ClaimAuthorisationService(IRepository<ClaimAuthorisation> repository, IRepository<IdentityRole> roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public ClaimAuthorisation Create(ClaimAuthorisation item)
        {
            return _repository.Add(item);
        }
        public ClaimAuthorisation Find(int id)
        {
            return _repository.Table().FirstOrDefault(x => x.Id == id);
        }
        public ClaimAuthorisation Update(ClaimAuthorisation item)
        {
            var user = _repository.Update(item);

            return user;
        }
        public ClaimAuthorisation Delete(ClaimAuthorisation item)
        {
            var user = _repository.Remove(item);

            return user;
        }
        public IQueryable<ClaimAuthorisation> GetQueryable()
        {
            return _repository.Table();
        }
        public IPagedList<ClaimAuthorisation> GetPageOf(int pagenumber, int pagesize)
        {
            return _repository.Table().OrderByDescending(x => x.Id).ToPagedList(pagenumber, pagesize);
        }
        public IQueryable<ClaimAuthorisation> GetTable()
        {
            return _repository.Table();
        }
        public List<AuthorisationResource> GetRoleAuthorisations(string roleName)
        {
            var roleAuthorisations = _repository.Table().Where(x => x.ClaimType == ClaimTypes.Role && x.Claim == roleName).ToList();
            var authorisationResources = GetAuthorisationResources();

            return LoadRoleAuthorisations(authorisationResources, roleAuthorisations);
        }
        public bool IsActionAuthorizedForRole(string roleName, string resourceName, string actionName)
        {
            return _repository.Table().Any(x => x.ClaimType == ClaimTypes.Role && x.Claim == roleName && x.Resource == resourceName && x.Action == actionName);
        }
        public bool CheckActionAuthorisationVariables(string roleName, string resourceName, string actionName)
        {
            var authorisationResources = GetAuthorisationResources();

            var authorisationResource = authorisationResources.FirstOrDefault(x => x.Name == resourceName);
            if (authorisationResource == null) return false;

            var authorisationAction = authorisationResource.Actions.FirstOrDefault(x => x.Name == actionName);
            if (authorisationAction == null) return false;

            var roleExists = _roleRepository.Table().Any(x => x.Name == roleName);
            return roleExists;
        }
        public ClaimAuthorisation FindRoleClaim(string roleName, string resourceName, string actionName)
        {
            return _repository.Table().FirstOrDefault(x => x.ClaimType == ClaimTypes.Role && x.Claim == roleName && x.Resource == resourceName && x.Action == actionName);
        }

        #region Helpers
        private List<AuthorisationResource> LoadRoleAuthorisations(List<AuthorisationResource> authorisationResources, List<ClaimAuthorisation> roleAuthorisations)
        {
            foreach (var resource in authorisationResources)
            {
                foreach (var action in resource.Actions)
                {
                    var a = action;

                    action.IsSelected = roleAuthorisations.Any(x => x.Resource == resource.Name && x.Action == a.Name);
                }
            }

            return authorisationResources;
        }
        private List<AuthorisationResource> GetAuthorisationResources()
        {
            var model = new List<AuthorisationResource>();

            // !!! UYARI: Bu veriler, kullanıcı yetkilerinin düzgün çalışabilmesi için kontrolsüz olarak değiştirilmemelidir !!!
            var basvurular = new AuthorisationResource
            {
                Name = Resources.Basvurular,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.BasvurularActions.View },
                new ResourceAction{ Name = Resources.BasvurularActions.Edit }
            }
            };

            model.Add(basvurular);

            var gonulluVericiMerkezi = new AuthorisationResource
            {
                Name = Resources.GonulluVericiMerkezi,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.GonulluVericiMerkeziActions.View },
                new ResourceAction{ Name = Resources.GonulluVericiMerkeziActions.Edit }
            }
            };

            model.Add(gonulluVericiMerkezi);

            var kullanicilar = new AuthorisationResource
            {
                Name = Resources.Kullanicilar,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.KullanicilarActions.View },
                new ResourceAction{ Name = Resources.KullanicilarActions.Edit }
            }
            };

            model.Add(kullanicilar);

            var guvenlikKayitlari = new AuthorisationResource
            {
                Name = Resources.GuvenlikKayitlari,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.GuvenlikKayitlariActions.View },
                new ResourceAction{ Name = Resources.GuvenlikKayitlariActions.Edit }
            }
            };

            model.Add(guvenlikKayitlari);

            var duyurular = new AuthorisationResource
            {
                Name = Resources.Duyurular,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.DuyurularActions.View },
                new ResourceAction{ Name = Resources.DuyurularActions.Edit }
            }
            };

            model.Add(duyurular);

            var dokumanlar = new AuthorisationResource
            {
                Name = Resources.Dokumanlar,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.DokumanlarActions.View },
                new ResourceAction{ Name = Resources.DokumanlarActions.Edit }
            }
            };

            model.Add(dokumanlar);

            var yardimMasasi = new AuthorisationResource
            {
                Name = Resources.YardimMasasi,
                Actions = new List<ResourceAction>
            {
                new ResourceAction{ Name = Resources.YardimMasasiActions.View },
                new ResourceAction{ Name = Resources.YardimMasasiActions.Edit }
            }
            };

            model.Add(yardimMasasi);

            return model;
        }
        #endregion
    }




}
