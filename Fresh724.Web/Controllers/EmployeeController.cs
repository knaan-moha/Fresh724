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
                var uploads = Path.Combine(wwwRootPath, @"images\product");
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
                employee.ImageUrl = @"\images\product\" + fileName + extension;

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
               var uploads = Path.Combine(wwwRootPath, @"images\product");
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
               employee.ImageUrl = @"\images\product\" + fileName + extension;

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
                    var uploads = Path.Combine(wwwRootPath, @"images\product");
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
                    employee.ImageUrl = @"\images\product\" + fileName + extension;

                }
                _unitOfWork.Employees.Remove(employee);
                _unitOfWork.SaveChanges();
                TempData["success"] = "Employee added successfully";
                return RedirectToAction("Index");

            }
            return View(employee);
        }
           
    
}


 /*

        // GET: Category
        public IActionResult Index()
        {
            IEnumerable<Employee> employee = _unitOfWork.Employees.GetAll();
            return View(employee);
        }


        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(Guid? id)
        {
            Employee employee = new();

            if (id == null )
            {
                return View(employee);
            }
            else
            {
                employee = _unitOfWork.Employees.GetFirstOrDefault(u => u.Id == id);
                return View(employee);
            }
        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(Guid? id,[Bind("Id, FirstName, LastName, ImageUrl,User, UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (employee.Id == null)
                    _unitOfWork.Employees.Add(employee);
                else
                    _unitOfWork.Employees.Update(employee);
                TempData["success"] = "Company updated successfully";
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
@model Fresh724.Entity.Entities.Employee

<div class="modal-header">
    <h1 class="modal-title fs-5">Create New User</h1>
    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
</div>
<div class="modal-body">
    <form id="frmAddNewUser" asp-action="AddOrEdit" method="post"
          data-ajax="true" data-ajax-method="post" enctype="multipart/form-data" data-ajax-update="#modalAddContent">
        <input asp-for="Id" hidden />

        <div asp-validation-summary="ModelOnly" class="text-danger small"></div>

        <div class="form-group mb-3">
            <label asp-for="FirstName" class="form-label">FirstName</label>
            <input asp-for="FirstName" class="form-control" placeholder="john doe"/>
            <span asp-validation-for="FirstName" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="LastName" class="form-label">LastName</label>
            <input asp-for="LastName" class="form-control" placeholder="johndoe"/>
            <span asp-validation-for="LastName" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="ImageUrl" class="form-label">ImageUrl</label>
            <input asp-for="ImageUrl" class="form-control" placeholder=""/>
            <span asp-validation-for="ImageUrl" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="UserId" class="form-label">UserId</label>
            <input asp-for="UserId" class="form-control" placeholder=""/>
            <span asp-validation-for="UserId" class="text-danger small"></span>
        </div>
    </form>
</div>
<div class="modal-footer">
    <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Close</button>
    <button type="submit" class="btn btn-primary btn-sm" form="frmAddNewUser"><i class="fa fa-save me-2"></i>Save</button>
</div>

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var employee =  _unitOfWork.Employees.GetFirstOrDefault(u => u.Id == id);
           
            if (employee == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
          
            if (employee != null)
            {
                _unitOfWork.Employees.Remove(employee);
            }

            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }*/
        
        /*
        
        public IActionResult Index()
    {
       
        var user = _um.GetUserAsync(User).Result;
        IEnumerable<Employee> employees = _unitOfWork.Employees.GetAll();
        return View(employees);
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult  Add()
    {
        var user = _um.GetUserAsync(User).Result;
        
        Employee employee = new();
        employee.CompanyId = user.CompanyId;
        employee.CreatedBy = user.CompanyName;
        
        if(employee == null)
        {
            return View();
        }
        
        
        return View(employee);
    }
   

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add([Bind("FirstName,LastName,CreatedBy,Company,CompanyId, CreatedDateTime,ImageUrl") ]Employee employee)
    {
        
       
        var user =  _um.GetUserAsync(User).Result;
        employee.CompanyId = user.CompanyId;
        employee.CreatedBy = user.CompanyName;
        employee.CreatedDateTime = DateTime.Now;

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\employees");
                var extension = Path.GetExtension(imageFile.FileName);

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
                    imageFile.CopyTo(fileStreams);
                }
                employee.ImageUrl = @"\images\employees\" + fileName + extension;
                
            }
            
            _unitOfWork.Employees.Add(employee);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
            
        }
        return View(employee);
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
        var employee=  _unitOfWork.Employees.GetFirstOrDefault(u=>u.Id==id);
        if (employee == null  || employee.CompanyId !=user.CompanyId)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
       
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, Employee employee)
    {

        var user = _um.GetUserAsync(User).Result;
        employee.CompanyId = user.CompanyId;
        employee.ModifiedBy = user.CompanyName;
        employee.ModifiedDateTime = DateTime.Now;

        if (ModelState.IsValid)
        {
            /*string wwwRootPath = _hostEnvironment.WebRootPath;
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\employees");
                var extension = Path.GetExtension(imageFile.FileName);

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
                    imageFile.CopyTo(fileStreams);
                }

                employee.ImageUrl = @"\images\employees\" + fileName + extension;
                
            }
            _unitOfWork.Employees.Update(employee);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
            
        }
        return View(employee);
    }

    // POST: Category/Delete/5
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
    }
            
            
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
                if (id == null)
                {
                    return NotFound();
                }
                var user = _um.GetUserAsync(User).Result;
                var employee = _unitOfWork.Employees.GetFirstOrDefault(u=>u.Id==id);
                if (employee == null || employee.CompanyId !=user.CompanyId )
                {
                    return NotFound();
                }

                return View(employee);
            }

// POST: Movies/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult Delete(Guid id,[Bind("Id, FirstName, LastName, ImageUrl, User, UserId") ]Employee employee)
            {
                
                var user = _um.GetUserAsync(User).Result;
                employee.CompanyId = user.CompanyId;
                if (ModelState.IsValid)
                {
                    _unitOfWork.Employees.Remove(employee);
                    _unitOfWork.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(employee);
            }
    */