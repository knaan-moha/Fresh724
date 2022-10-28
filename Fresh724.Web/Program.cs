using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fresh724.Data;
using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Data.Repository.Concrete;
using Fresh724.Entity.Entities;
using ApplicationDbInitializer = Fresh724.Data.ApplicationDbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();*/

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllersWithViews();


var app = builder.Build();

using (var services =app.Services.CreateScope())
{
    var db = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var um  = services.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var rm  =  services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    ApplicationDbInitializer.Initialize(db,um,rm);
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=AddressUser}/{action=Add}/{id?}");
app.MapRazorPages();

app.Run();



/*
 {
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Fresh724;Trusted_connection=True;"
  },
 */