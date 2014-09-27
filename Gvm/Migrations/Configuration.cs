using Gvm.Infra;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Model;

namespace Gvm.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Gvm.Infra.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Gvm.Infra.DataContext";
        }

        protected override void Seed(Gvm.Infra.DataContext context)
        {
            CreateRoles(context);
            CreateAdmin(context);

            for (var i = 1; i < 10; i++)
            {
                CreateTestUser(context, "testUser" + i + "@turkok.com");
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }

        private void CreateRoles(DataContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string[] roleNames = { "admin", "Yönetici", "Ýl Yöneticisi", "Ýl Gözlemcisi", "Ýl Sorumlusu" };

            foreach (var roleName in roleNames)
            {
                if (roleManager.RoleExists(roleName) == false)
                {
                    var role = new IdentityRole { Name = roleName };

                    roleManager.Create(role);
                }
            }
        }
        private void CreateAdmin(DataContext context)
        {
            const string userEmail = "admin@turkok.com";
            const string userName = userEmail;
            const string userPassword = "atagun";
            const string userFullName = "Müdür";
            const string userRole = "Yönetici";
            
            var userManager = new UserManager<ApplicationUser>(new ApplicationUserStore<ApplicationUser>(context));
            var user = userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userEmail,
                    DateCreated = DateTime.Now,
                    FullName = userFullName,
                    UserRole = userRole,
                    Active = true
                };

                userManager.Create(user, userPassword);
                userManager.AddToRole(user.Id, userRole);
            }

            var userInRole = userManager.IsInRole(user.Id, userRole);

            if (userInRole == false)
            {
                userManager.AddToRole(user.Id, userRole);
            }
        }
        private void CreateTestUser(DataContext context, string userName)
        {
            const string userPassword = "test12!";

            var userManager = new UserManager<ApplicationUser>(new ApplicationUserStore<ApplicationUser>(context));
            var user = userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DateCreated = DateTime.Now,
                    FullName = userName,
                    UserRole = null,
                    Active = false
                };

                userManager.Create(user, userPassword);
            }
        }
    }
}
