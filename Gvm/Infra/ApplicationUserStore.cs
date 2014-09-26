using System.Data.Entity;
using Gvm.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Core.Repository;
using Turkok.Model;

namespace Gvm.Infra
{
    public class ApplicationUserStore<TUser> : UserStore<TUser> where TUser : ApplicationUser
    {
        private readonly IDbContext _context;

        public ApplicationUserStore(IDbContext context) : base((DbContext) context)
        {
            _context = context;
        }
    }
}