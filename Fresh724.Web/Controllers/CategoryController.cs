using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Fresh724.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace Fresh724.Web.Controllers;
[Authorize(Roles = "Admin")]

public class CategoryController : Controller
{
    private readonly ILogger<CategoryController > _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _um;
    private readonly IWebHostEnvironment _hostEnvironment;

    public CategoryController(ILogger<CategoryController > logger,IWebHostEnvironment hostEnvironment,IUnitOfWork unitOfWork,ApplicationDbContext db, UserManager<ApplicationUser> um)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _um = um;
        _db = db;
        _hostEnvironment= hostEnvironment;
    }
    
    public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
    {
        ViewBag.CurrentSort = sortOrder;
        ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

        if (searchString != null)
        {
            page = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewBag.CurrentFilter = searchString;
        
        var categories = from s in _unitOfWork.Categories.GetAll()  
                       select s;
        if (!String.IsNullOrEmpty(searchString))
        {
            categories = categories.Where(s => s.Name.Contains(searchString)
                                           || s.ImageUrl.Contains(searchString));
        }
        switch (sortOrder)
        {
            case "name_desc":
                categories = categories.OrderByDescending(s => s.Name);
                break;
            case "Date":
                categories = categories.OrderBy(s => s.CreatedDateTime);
                break;
            case "date_desc":
                categories = categories.OrderByDescending(s => s.CreatedDateTime);
                break;
            default:  // Name ascending 
                categories = categories.OrderBy(s => s.Name);
                break;
        }

        int pageSize = 2;
        int pageNumber = (page ?? 1);
        return View(categories.ToPagedList(pageNumber, pageSize));
    }
    
    [Authorize]
    [HttpGet]

    public IActionResult Add()
    {
        Category category = new Category();
        var user = _um.GetUserAsync(User).Result;
        category.CreatedBy = user.UserName;
        category.CreatedDateTime = DateTime.Now;
        category.Status="Active";
        return View(category);
    }
    //[Bind("Id, Name, Status, CreateDateTime,ImageUrl, User, UserId") ]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("Id, Name, Status, CreateDateTime,ImageUrl, User, UserId") ]Category category, IFormFile? file)
    {
        category.CreatedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        category.CreatedBy = user.UserName;
        category.CreatedDateTime = DateTime.Now;
        category.Status="Active";
        ModelState.Clear();

        // Reevaluate the model with the added fields
        if (TryValidateModel(category))
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\category");
                var extension = Path.GetExtension(file.FileName);

                if (category.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, category.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                category.ImageUrl = @"\images\category\" + fileName + extension;

            }
            _unitOfWork.Categories.Add(category);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Category Added successfully";
            return RedirectToAction("Index");
        }
        return View(category);   
    }
    
   
 
    [Authorize]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = _um.GetUserAsync(User).Result;
      
       
        //var posts = await _db.Posts.FirstOrDefaultAsync(m=>m.Id==id);
        var category=  _unitOfWork.Categories.GetFirstOrDefault(u=>u.Id==id);
        if (category == null )//|| category.UserId !=user.Id )
        {
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }
    
   
 
   
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Edit([Bind("Id, Name, Status, CreateDateTime,ImageUrl, User, UserId") ]Category category, IFormFile? file)
   {
       category.CreatedDateTime = DateTime.Now;
       var user = _um.GetUserAsync(User).Result;
       //category.User = user;
       //category.UserId = user.Id ;
       ModelState.Clear();

       // Reevaluate the model with the added fields
       if (TryValidateModel(category))
       
       {
           string wwwRootPath = _hostEnvironment.WebRootPath;
           if (file != null)
           {
               string fileName = Guid.NewGuid().ToString();
               var uploads = Path.Combine(wwwRootPath, @"images\category");
               var extension = Path.GetExtension(file.FileName);

               if (category.ImageUrl != null)
               {
                   var oldImagePath = Path.Combine(wwwRootPath, category.ImageUrl.TrimStart('\\'));
                   if (System.IO.File.Exists(oldImagePath))
                   {
                       System.IO.File.Delete(oldImagePath);
                   }
               }

               using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
               {
                   file.CopyTo(fileStreams);
               }
               category.ImageUrl = @"\images\category\" + fileName + extension;

           }
           _unitOfWork.Categories.Update(category);
           _unitOfWork.SaveChanges();
           TempData["success"] = "Category Edited successfully";
           return RedirectToAction("Index");
       }

       return View(category);
   }

   [Authorize]
   [HttpGet]
   public IActionResult Delete(Guid? Id)
   {
       if (Id == null)
       {
           return NotFound();
       }
       var user = _um.GetUserAsync(User).Result;
       var category = _unitOfWork.Categories.GetFirstOrDefault(u=>u.Id==Id);
       if (category == null ) 
       {
           return NotFound();
       }

       return View(category);
   }

// POST: Movies/Delete/5
   [HttpPost, ActionName("Delete")]
   [ValidateAntiForgeryToken]
   public IActionResult Delete(Guid id, Category category)
   {
       category.CreatedDateTime = DateTime.Now;
       var user = _um.GetUserAsync(User).Result;
       
       var category1 =  _unitOfWork.Categories.GetFirstOrDefault(u => u.Id == id);
       _unitOfWork.Categories.Remove(category1);
       _unitOfWork.SaveChanges();
       TempData["success"] = "Category deleted successfully";
       return RedirectToAction("Index");
   }
  

}