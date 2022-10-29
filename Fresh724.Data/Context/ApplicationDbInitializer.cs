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
            
            var admin = new ApplicationUser
                { UserName = "Admin@fresh.no", Email = "Admin@fresh.no", EmailConfirmed = true };
            
            var user = new ApplicationUser { UserName = "user@uia.no", Email = "user@uia.no", EmailConfirmed = true };
            um.CreateAsync(admin, "Password1.").Wait();

            um.AddToRoleAsync(admin, "Admin").Wait();
            um.CreateAsync(user, "Password1.").Wait();


            db.SaveChanges();



        }
    }
}


