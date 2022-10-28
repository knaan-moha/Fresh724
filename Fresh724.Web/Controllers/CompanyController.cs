using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fresh724.Web.Controllers;

[Authorize(Roles = "Admin")]
// GET
public class CompanyController : Controller
{
    
    private readonly ILogger<CompanyController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _um;
 

    public CompanyController(ILogger<CompanyController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationUser> um)
    {
        
        _logger = logger;
        _unitOfWork = unitOfWork;
        _um = um;
        
    }
    
    public IActionResult Index()
    {
       
        var user = _um.GetUserAsync(User).Result;
        //IEnumerable<Company> company = _unitOfWork.Companies.GetByFilter( x => x.UserId == user.Id);
        IEnumerable<Company> company = _unitOfWork.Companies.GetAll();
        return View(company);
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult  Add()
    {
        var user = _um.GetUserAsync(User).Result;
        
        Company company = new();
        company.CreatedBy = user.UserName;
        if(company == null)
        {
            return View();
        }
        
            
        
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add([Bind("Name,Status, User, UserId") ]Company company)
    {
        
        var user = _um.GetUserAsync(User).Result;
        company.CreatedBy = user.UserName;


        if (ModelState.IsValid)
        {
            _unitOfWork.Companies.Add(company);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Company added successfully";
            return RedirectToAction("Index");
        }
        else
        {
            return View(company);
        }
        
    }
    
    
    [Authorize]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var company = _unitOfWork.Companies.GetFirstOrDefault(u=>u.Id==id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (company == null)
        {
            return NotFound();
        }

        return View(company);
       
    }
    
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id,[Bind("Id, Name, User, UserId") ]Company company)
    {
        company.ModifiedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        company.ModifiedBy = user.UserName;
        
        if (ModelState.IsValid)
        {
            _unitOfWork.Companies.Update(company);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(company);
    }
    
    
            /*// POST: Category/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult Delete(Guid Id)
            {
                if (_unitOfWork.Companies == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
                }
                var company =  _unitOfWork.Companies.GetFirstOrDefault(u => u.Id == Id);
                if (company != null)
                {
                    _unitOfWork.Companies.Remove(company);
                }
    
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }*/
            
            
            [HttpGet]
            public IActionResult GetAll()
            {
                var companyList = _unitOfWork.Companies.GetAll();
                return Json(new { data = companyList });
            }

            [Authorize]
            [HttpGet]
            public IActionResult Delete(Guid? id)
            {
                var company = _unitOfWork.Companies.GetFirstOrDefault(u=>u.Id==id);
                //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

                if (company == null)
                {
                    return NotFound();
                }

                return View(company);
            }

// POST: Movies/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult Delete(Guid id,[Bind("Id, Name, User, UserId") ]Company company)
            {
                company.CreatedDateTime = DateTime.Now;
                var user = _um.GetUserAsync(User).Result;
                if (ModelState.IsValid)
                {
                    _unitOfWork.Companies.Remove(company);
                    _unitOfWork.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(company);
            }
    
    
}