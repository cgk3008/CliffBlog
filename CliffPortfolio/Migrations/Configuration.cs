namespace CliffPortfolio.Migrations
{
    using CliffPortfolio.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CliffPortfolio.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true; used this to get past Update-Database "Automatic migration was not applied because it would result in data loss. "
        }

        protected override void Seed(CliffPortfolio.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(
     new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "cgk3008.ck@gmail.com, araynor@coderfoundry.com"))   // this line is saying if user does not exist with this email, then create that user????
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "cgk3008.ck@gmail.com",
                    Email = "cgk3008.ck@gmail.com",
                    FirstName = "Cliff",
                    LastName = "Koenig",
                    DisplayName = "CK"
                }, "Redd12!");

                userManager.Create(new ApplicationUser
                {
                    UserName = "araynor@coderfoundry.com",
                    Email = "araynor@coderfoundry.com",
                    FirstName = "Antonio",
                    LastName = "Raynor",
                    DisplayName = "Antonio"
                }, "OurTeacher12!");
            }
            
            var userId = userManager.FindByEmail("cgk3008.ck@gmail.com").Id;  //forgot to add my email on this line and "Object reference not set to an instance of an object." showed up in Package Manager Console
            userManager.AddToRole(userId, "Admin");

            var userIdMod = userManager.FindByEmail("araynor@coderfoundry.com").Id;  
            userManager.AddToRole(userIdMod, "Moderator");


        }
    }
}
