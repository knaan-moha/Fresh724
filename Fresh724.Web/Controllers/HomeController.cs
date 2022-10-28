using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Fresh724.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _um;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationUser> um)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _um = um;
    }

    public IActionResult Index()
    {
      //  var user = _um.GetUserAsync(User).Result;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
}