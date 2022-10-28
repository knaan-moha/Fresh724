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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace Fresh724.Web.Controllers;

[Authorize(Roles = "Admin,Company")]

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _um;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ApplicationDbContext _db;

    public ProductController(ILogger<ProductController> logger,ApplicationDbContext db, IUnitOfWork unitOfWork,
        IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> um)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
        _um = um;
        _db = db;
    }

    public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
    {
        ViewBag.CurrentSort = sortOrder;
        ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
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

        var products = from s in _unitOfWork.Products.GetAll()
            select s;
        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(s => s.Title.Contains(searchString)
                                           || s.ImageUrl.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "title_desc":
                products = products.OrderByDescending(s => s.Title);
                break;
            case "Date":
                products = products.OrderBy(s => s.CreatedDateTime);
                break;
            case "date_desc":
                products = products.OrderByDescending(s => s.CreatedDateTime);
                break;
            default: // Name ascending 
                products = products.OrderBy(s => s.Title);
                break;

        }

        int pageSize = 2;
        int pageNumber = (page ?? 1);
        return View(products.ToPagedList(pageNumber, pageSize));
    }



    //GET
    public IActionResult Add()
    {
        var user = _um.GetUserAsync(User).Result;
        ProductViewEntity product = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };
        product.Product.CompanyId = user.CompanyId;
        product.Product.CreatedBy = user.CompanyName;
        product.Product.CreatedDateTime = DateTime.Now;
        product.Product.Status = "Active";
        return View(product);

    }


    //[Bind("Id, Name, Status, CreateDateTime,ImageUrl, User, UserId") ]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        ProductViewEntity product, IFormFile? file)
    {
        product.Product.CreatedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        product.Product.CreatedBy = user.UserName;
        product.Product.CreatedDateTime = DateTime.Now;
        

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\product");
                var extension = Path.GetExtension(file.FileName);

                if ( product.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath,  product.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                product.Product.ImageUrl = @"\images\product\" + fileName + extension;

            }

            _unitOfWork.Products.Add(product.Product);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Category Added successfully";
            return RedirectToAction("Index");
        }

        return View(product);
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
        ProductViewEntity product = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };
        product.Product.CompanyId = user.CompanyId;
        product.Product.ModifiedBy = user.CompanyName;
        product.Product.ModifiedDateTime = DateTime.Now;
        return View(product);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ProductViewEntity product, IFormFile? file)
    {
        product.Product.CreatedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        product.Product.ModifiedBy = user.CompanyName;
        product.Product.ModifiedDateTime = DateTime.Now;
        

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\product");
                var extension = Path.GetExtension(file.FileName);

                if ( product.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath,  product.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                product.Product.ImageUrl = @"\images\product\" + fileName + extension;

            }

            _unitOfWork.Products.Update(product.Product);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Category Added successfully";
            return RedirectToAction("Index");
        }

        return View(product);
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
        var product = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == Id);
        if (product == null) //|| category.UserId !=user.Id )
        {
            return NotFound();
        }

        return View(product);
    }

// POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var user = _um.GetUserAsync(User).Result;
        
        var product1 = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
        _unitOfWork.Products.Remove(product1);
        _unitOfWork.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }

}
/*
 
 
 
   //GET
public IActionResult Add(Guid? id)
{
    var user = _um.GetUserAsync(User).Result;
    ProductViewEntity product = new()
    {
        Product = new(),
        CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        }),
    };
    product.Product.CompanyId = user.CompanyId;
    product.Product.CreatedBy = user.CompanyName;
    product.Product.CreatedDateTime = DateTime.Now;
    if (id == null || id == Guid.Empty)
    {
        
        return View(product);
    }
    else
    {
        
        product.Product = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
        return View(product);
        
    }


}

[HttpPost]
[ValidateAntiForgeryToken]
// [Bind("Product.Title, Product.CreatedDateTime,Product.CreatedBy,Product.ImageUrl,Product.Company.CompanyName, Product.Description,Product.PurchasePrice,Product.Status, Product.Category.CategoryName, Product.Category, Product.CategoryId, Product.Company, Product.CompanyId,  User, UserId") ]
//POST
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult AddOrEdit(Guid id,ProductViewEntity item, IFormFile? file)
{
    var user =  _um.GetUserAsync(User).Result;
    item.Product.CompanyId = user.CompanyId;
    item.Product.CreatedBy = user.CompanyName;
    item.Product.CreatedDateTime = DateTime.Now;

    if (!ModelState.IsValid)
    {
        string wwwRootPath = _hostEnvironment.WebRootPath;
        if (file != null)
        {
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"images\product");
            var extension = Path.GetExtension(file.FileName);

            if (item.Product.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, item.Product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                file.CopyTo(fileStreams);
            }
            item.Product.ImageUrl = @"\images\product\" + fileName + extension;

        }
        if (item.Product.Id == Guid.Empty)
        {
            _unitOfWork.Products.Add(item.Product);
        }
        else
        {
            _unitOfWork.Products.Update(item.Product);
        }
        _unitOfWork.SaveChanges();
        TempData["success"] = "Product created successfully";
        return RedirectToAction("Index");
    }
    return View(item);
}

[HttpGet]
public IActionResult GetAll()
{
    var productList = _unitOfWork.Products.GetAll(includeProperties: "Category");
    return Json(new { data = productList });
}

//POST
[HttpDelete]
public IActionResult Delete(Guid? id)
{
    var obj = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
    if (obj == null)
    {
        return Json(new { success = false, message = "Error while deleting" });
    }

    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
    if (System.IO.File.Exists(oldImagePath))
    {
        System.IO.File.Delete(oldImagePath);
    }

    _unitOfWork.Products.Remove(obj);
    _unitOfWork.SaveChanges();
    return Json(new { success = true, message = "Deleted Successfully" });

}

}


[AllowAnonymous]
public IActionResult Index()
{
    IEnumerable<Product> product= _unitOfWork.Products.GetAll();
    return View(product);
}

[Authorize]
[HttpGet]

public IActionResult Add()
{
   ProductViewEntity product = new()
    {
        
        CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
        {
            Text = i.Name,
            Value = i.Id.ToString()
        }),
    };
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Add([Bind("Id, Title, CreateDateTime,ImageUrl, Description,PurchasePrice,Status, User, UserId") ] ProductViewEntity product)
{
    var user = await _um.GetUserAsync(User);
    product.CompanyId = user.CompanyId;
    product.CreatedDateTime = DateTime.Now;
    if (ModelState.IsValid)
    {
      
        _unitOfWork.Products.Add(product);
        _unitOfWork.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
    return View(product);
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
    var product=  _unitOfWork.Products.GetFirstOrDefault(u=>u.Id==id);
    if (product == null  || product.CompanyId !=user.CompanyId)
    {
        return RedirectToAction(nameof(Index));
    }
    return View(product);
}




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit([Bind("Id, Title, CreateDateTime,ImageUrl, Description,PurchasePrice,Status, User, UserId") ]Product product)
{
   product.CreatedDateTime = DateTime.Now;
   var user = _um.GetUserAsync(User).Result;
   product.CompanyId = user.CompanyId;
    
    
   if (!ModelState.IsValid)
   {
       _unitOfWork.Products.Update(product);
       _unitOfWork.SaveChanges();
       TempData["success"] = "Category Edited successfully";
       return RedirectToAction("Index");
   }

   return View(product);
}

@foreach (var product in Model)
                             {
                                 <tbody>
                                 <tr class="table table-secondary">
                                     <td class="Title">@product.Product.Title</td>
                                     <td class="CategoryName">@product.Product.Category.Name</td>
                                     <td class="CreatedDateTime">@product.Product.CreatedDateTime</td>
                                     <td class="PurchasePrice">@product.Product.PurchasePrice</td>
                                     <td class="Company.Name">@product.Product.Company.Name</td>
                                     <td><a asp-action="AddOrEdit" asp-route-id="@product.Product.Id" class="edit" title="Edit" data-toggle="tooltip"><i class="material-icons">&#xE254;</i></a> </td>
                                     <td><a asp-action="Delete" asp-route-id="@product.Product.Id"  onclick="return confirm('Are you sure want to delete this user : @product.Product.Title ?')" class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a>
                                     </td>
                                 </tr>
                                 </tbody>
                                 
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
   var product = _unitOfWork.Products.GetFirstOrDefault(u=>u.Id==id);
   if (product == null || product.CompanyId !=user.CompanyId )
   {
       return NotFound();
   }

   return View(product);
}

// POST: Movies/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult Delete(Guid id, [Bind("Id, Title, CreateDateTime,ImageUrl, Description,PurchasePrice,Status, User, UserId") ]Product product)
{
   product.CreatedDateTime = DateTime.Now;
   var user = _um.GetUserAsync(User).Result;
   //category.User = user;
   product.CompanyId = user.CompanyId;
   
   
   var product1 =  _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
   _unitOfWork.Products.Remove(product1);
   _unitOfWork.SaveChanges();
   TempData["success"] = "Category deleted successfully";
   return RedirectToAction("Index");
}*/