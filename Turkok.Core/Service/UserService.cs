using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryCacheT;
using Microsoft.AspNet.Identity;
using Turkok.Core.Repository;
using Turkok.Core.Service.CrudService;
using PagedList;
using Turkok.Model;

namespace Turkok.Core.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<ApplicationUser> _repository;
        private readonly UserManager<ApplicationUser> _userManager;        
        static readonly TimeSpan Expiration = TimeSpan.FromHours(1);
        private static readonly MemoryCacheT.Cache<string, ApplicationUser> Cache = new Cache<string, ApplicationUser>(Expiration);

        public UserService(IRepository<ApplicationUser> repository, UserManager<ApplicationUser> usermanager)
        {
            _repository = repository;
            _userManager = usermanager;
        }

        public ApplicationUser GetCurrentUserFromDatabase(string userName)
        {
            var user = _userManager.FindByName(userName);

            return user;
        }
        public ApplicationUser GetCurrentUser(string userName)
        {
            ApplicationUser user = null;

            if (Cache.TryGetValue(userName, out user) == false)
            {
                user = _userManager.FindByNameAsync(userName).Result;
                Cache.Add(userName, user);
            }

            return user;
        }
        public ApplicationUser Create(ApplicationUser item)
        {
            var result = _userManager.Create(item);

            if (result.Succeeded)
            {

            }

            return null;
        }
        public ApplicationUser InviteUser(string creator, string roleName, ApplicationUser user)
        {
            //var password = "123456";
            //var userInDb = _userManager.FindByNameAsync(user.UserName).Result;

            //if (userInDb == null)
            //{
            //    user.Creator = creator;
            //    user.Invitation = InvitationStatus.Invited;
            //    user.ConfirmationCode = Guid.NewGuid();
            //    user.DateCreated = DateTime.Now;

            //    var idResult = _userManager.Create(user, password);

            //    if (idResult.Succeeded)
            //    {
            //        _userManager.AddToRole(user.Id, roleName);

            //        return user;
            //    }
            //}

            return null;
        }
        public ApplicationUser Find(int id)
        {
            var user = _repository.Find(id);

            return user;
        }
        public ApplicationUser Find(string userId)
        {
            var user = _repository.Table().FirstOrDefault(x => x.Id == userId);

            return user;
        }
        public ApplicationUser Update(ApplicationUser item)
        {
            var user = _repository.Update(item);

            return user;
        }
        public ApplicationUser Delete(ApplicationUser item)
        {
            var user = _repository.Remove(item);

            return user;
        }
        public void ChangePassword(string userId, string newPassword)
        {
            _userManager.RemovePassword(userId);
            _userManager.AddPassword(userId, newPassword);
        }
        public void ChangePassword(string userId, string oldPassword, string newPassword)
        {
            _userManager.ChangePassword(userId, oldPassword, newPassword);
        }
        public IQueryable<ApplicationUser> GetQueryable()
        {
            var users = _repository.Table();
            return users;
        }
        public IPagedList<ApplicationUser> GetPageOf(int pagenumber, int pagesize)
        {
            var users = _repository.Table().OrderByDescending(x => x.Id).ToPagedList(pagenumber, pagesize);

            return users;
        }
        private IQueryable<ApplicationUser> GetQueryResult(IQueryable<ApplicationUser> usersQuery, string query)
        {
            if (string.IsNullOrEmpty(query)) return usersQuery;

            var queryParts = query.Split(' ');

            foreach(var item in queryParts)
            {
                if (string.IsNullOrEmpty(item)) continue;

                var ql = item.Trim().ToLower();
                var qu = item.Trim().ToUpper();

                usersQuery = usersQuery.Where(x => x.Email.ToLower().Contains(ql) || x.Email.ToUpper().Contains(qu) || x.FullName.ToLower().Contains(ql) || x.FullName.ToUpper().Contains(qu) || x.UserName.ToLower().Contains(ql) || x.UserName.ToUpper().Contains(qu) || x.WorkPhone.ToLower().Contains(ql) || x.CellPhone.ToLower().Contains(ql));
            }
            
            return usersQuery;
        }
        public IQueryable<ApplicationUser> GetTable()
        {
            return _repository.Table();
        }        
    }
}
