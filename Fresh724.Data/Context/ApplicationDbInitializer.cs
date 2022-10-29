using System.Security.Principal;
using Fresh724.Data.Context;
using Fresh724.Entity.Entities;
using Fresh724.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fresh724.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> rm)
    {
        // Delete the database before we initialize it. This is common to do during development.
        // Delete the database before we initialize it. This is common to do during development.
        //db.Database.EnsureDeleted();
        // Recreate the database and tables according to our models
        db.Database.EnsureCreated();
        // Add test data to simplify debugging and testing

        if (!rm.RoleExistsAsync(RoleService.Role_Admin).GetAwaiter().GetResult())
        {
            rm.CreateAsync(new IdentityRole(RoleService.Role_Admin)).GetAwaiter().GetResult();
            rm.CreateAsync(new IdentityRole(RoleService.Role_Employee)).GetAwaiter().GetResult();
            rm.CreateAsync(new IdentityRole(RoleService.Role_User_Indi)).GetAwaiter().GetResult();
            rm.CreateAsync(new IdentityRole(RoleService.Role_User_Comp)).GetAwaiter().GetResult();

            //if roles are not created, then we will create admin user as well
            
            var company1= new Company
            {
                Name = "YUNUS",
                Status = "Active",
                CreatedDateTime = DateTime.Now,
                CreatedBy = "System",
            };
            
            
            var admin = new ApplicationUser
                { UserName = "Yunusy@uia.no", Email = "Yunusy@uia.no", EmailConfirmed = true };

            var company = new ApplicationUser
                { UserName = "georgetell22@gmail.com", Email = "georgetell22@gmail.com", CompanyName = "YUNUS",EmailConfirmed = true };
            um.CreateAsync(company, "1215Yns.").Wait();
            um.AddToRoleAsync(company, "Company").Wait();
            
            var user = new ApplicationUser { UserName = "user@uia.no", Email = "user@uia.no", EmailConfirmed = true };
            um.CreateAsync(admin, "Password1.").Wait();

            um.AddToRoleAsync(admin, "Admin").Wait();
            um.CreateAsync(user, "Password1.").Wait();


            db.SaveChanges();



        }
    }
}


