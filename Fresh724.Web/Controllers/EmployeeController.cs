using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Fresh724.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace Fresh724.Web.Controllers;

[Authorize(Roles = "Company")]
// GET
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _um;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(ILogger<EmployeeController> logger, IUnitOfWork unitOfWork,IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> um)
        {
        
            _logger = logger;
            _unitOfWork = unitOfWork;
            _um = um;
            _hostEnvironment = hostEnvironment;
        
        }
       
        
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
       
        ViewBag.CurrentSort = sortOrder;
        ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
        ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "comp_desc" : "";
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

        var employee = from s in _unitOfWork.Employees.GetAll()
            select s;
        if (!string.IsNullOrEmpty(searchString))
        {
            employee = employee.Where(s => s.FirstName.Contains(searchString)
                                           || s.ImageUrl.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "first_desc":
                employee = employee.OrderByDescending(s => s.FirstName);
                break;
            case "comp_desc":
                employee = employee.OrderByDescending(s => s.CompanyName);
                break;
            case "Date":
                employee = employee.OrderBy(s => s.CreatedDateTime);
                break;
            case "date_desc":
                employee = employee.OrderByDescending(s => s.CreatedDateTime);
                break;
            default: // Name ascending 
                employee = employee.OrderBy(s => s.FirstName);
                break;

        }

        int pageSize = 2;
        int pageNumber = (page ?? 1);
        return View(employee.ToPagedList(pageNumber, pageSize));
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult  Add()
    {
        var user = _um.GetUserAsync(User).Result;
        Employee employee = new();
        employee.CreatedBy = user.CompanyName;
        employee.CompanyName = user.CompanyName;
        employee.CreatedDateTime = DateTime.Now;
        if(employee == null)
        {
            return View();
        }
        
            
        
        return View(employee);
    }
//[Bind("FirstName,LastName,CreatedBy,CompanyName, CreatedDateTime,ImageUrl, User, UserId") ]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(Employee employee,IFormFile? file)
    {

        var user = _um.GetUserAsync(User).Result;
        employee.CreatedBy = user.CompanyName;
        employee.CompanyName = user.CompanyName;
        employee.CreatedDateTime = DateTime.Now;
        ModelState.Clear();

        // Reevaluate the model with the added fields
        if (TryValidateModel(employee))
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\employee");
                var extension = Path.GetExtension(file.FileName);

                if (employee.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, employee.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                employee.ImageUrl = @"\images\employee\" + fileName + extension;

            }
            _unitOfWork.Employees.Add(employee);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Employee added successfully";
            return RedirectToAction("Index");

        }
        return View(employee);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    { 
        var employee = _unitOfWork.Employees.GetFirstOrDefault(u=>u.Id==id);
        employee.ModifiedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        employee.ModifiedBy = user.CompanyName;
        
        if (id == null)
        {
            return NotFound();
        }
        
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
       
    }
    
   //[Bind("FirstName,LastName,CreatedBy,CompanyName, CreatedDateTime,ImageUrl, User, UserId") ]
   public IActionResult Edit(Employee employee,IFormFile? file)
   {

       var user = _um.GetUserAsync(User).Result;
       employee.ModifiedBy = user.CompanyName;
       employee.CompanyName = user.CompanyName;
       employee.ModifiedDateTime = DateTime.Now;
       ModelState.Clear();

       // Reevaluate the model with the added fields
       if (TryValidateModel(employee))
       {
           string wwwRootPath = _hostEnvironment.WebRootPath;
           if (file != null)
           {
               string fileName = Guid.NewGuid().ToString();
               var uploads = Path.Combine(wwwRootPath, @"images\employee");
               var extension = Path.GetExtension(file.FileName);

               if (employee.ImageUrl != null)
               {
                   var oldImagePath = Path.Combine(wwwRootPath, employee.ImageUrl.TrimStart('\\'));
                   if (System.IO.File.Exists(oldImagePath))
                   {
                       System.IO.File.Delete(oldImagePath);
                   }
               }

               using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
               {
                   file.CopyTo(fileStreams);
               }
               employee.ImageUrl = @"\images\employee\" + fileName + extension;

           }
           _unitOfWork.Employees.Update(employee);
           _unitOfWork.SaveChanges();
           TempData["success"] = "Employee added successfully";
           return RedirectToAction("Index");

       }
       return View(employee);
   }
   

            [Authorize]
            [HttpGet]
            public IActionResult Delete(Guid? id)
            {
                var employee = _unitOfWork.Employees.GetFirstOrDefault(u=>u.Id==id);
                //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
//[Bind("FirstName,LastName,CreatedBy,CompanyName, CreatedDateTime,ImageUrl, User, UserId") ]
// POST: Movies/Delete/5
        public IActionResult Delete(Employee employee,IFormFile? file)
        {

            var user = _um.GetUserAsync(User).Result;
            employee.CompanyName = user.CompanyName;
            ModelState.Clear();

            // Reevaluate the model with the added fields
            if (TryValidateModel(employee))
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\employee");
                    var extension = Path.GetExtension(file.FileName);

                    if (employee.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, employee.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    employee.ImageUrl = @"\images\employee\" + fileName + extension;

                }
                _unitOfWork.Employees.Remove(employee);
                _unitOfWork.SaveChanges();
                TempData["success"] = "Employee added successfully";
                return RedirectToAction("Index");

            }
            return View(employee);
        }
        
        //Get Details
        [Authorize]
        [HttpGet]       
        public IActionResult Details(Guid? id)
        {
            var employee = _unitOfWork.Employees.GetFirstOrDefault(u=>u.Id==id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
    
}


        