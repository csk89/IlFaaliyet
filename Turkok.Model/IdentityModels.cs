using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Model.Lookup;

namespace Turkok.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    { 
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [NotMapped]
        public ICollection<Claim> DbClaims { get; set; }
        public DateTime DateCreated { set; get; }
        public int? UnitId { get; set; }
        public int? CityId { get; set; }
        public string TcKimlikNo { get; set; }
        public string FullName{ get; set; }
        public Gender? Gender { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public bool NotWorkingForUnit { get; set; }
        public UserScope? Scope { get; set; }
        public RoleInUnit? RoleInUnit { get; set; }
        public string UserRole { get; set; }
        public bool Active { get; set; }
        public string LastLogin { set; get; }
    }

}