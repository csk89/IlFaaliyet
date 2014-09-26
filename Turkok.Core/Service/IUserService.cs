using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Turkok.Core.Service.CrudService;
using PagedList;
using Turkok.Model;

namespace Turkok.Core.Service
{
    public interface IUserService : ICrudService<ApplicationUser>
    {
        ApplicationUser InviteUser(string creator, string roleName, ApplicationUser user);
        void ChangePassword(string userId, string newPassword);
        void ChangePassword(string userId, string oldPassword, string newPassword);
        ApplicationUser GetCurrentUser(string username);
        ApplicationUser GetCurrentUserFromDatabase(string userName);
        ApplicationUser Find(string userId);    
    }
}
