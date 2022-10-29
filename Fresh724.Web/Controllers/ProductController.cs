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
using X.PagedList;
using X.PagedList.Mvc.Core;

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
        ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
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

        var products = from s in _unitOfWork.Products.GetAll()
            select s;
        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(s => s.Title.Contains(searchString)
                                           || s.Description.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "title_desc":
                products = products.OrderByDescending(s => s.Title);
                break;
            case "price_desc":
                products = products.OrderByDescending(s => s.PurchasePrice);
                break;
            case "comp_desc":
                products = products.OrderByDescending(s => s.CompanyId);
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
       public IActionResult AddOrEdit(Guid? id)
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
            product.Product.ModifiedBy = user.CompanyName;
            product.Product.ModifiedDateTime = DateTime.Now;
            product.Product = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
            return View(product);
            
        }


    }

   // [Bind("Product.Title, Product.CreatedDateTime,Product.CreatedBy,Product.ImageUrl,Product.Company.CompanyName, Product.Description,Product.PurchasePrice,Product.Status, Product.Category.CategoryName, Product.Category, Product.CategoryId, Product.Company, Product.CompanyId,  User, UserId") ]
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddOrEdit(Guid id,ProductViewEntity item, IFormFile? file)
    {
        var user =  _um.GetUserAsync(User).Result;
        item.Product.CompanyId = user.CompanyId;
        item.Product.CategoryName = _unitOfWork.Categories.GetFirstOrDefault(u => u.Id == item.Product.CategoryId).Name;
        ModelState.Clear();

        // Reevaluate the model with the added fields
        if (TryValidateModel(file))
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
                item.Product.CreatedBy = user.CompanyName;
                item.Product.CreatedDateTime = DateTime.Now;
                _unitOfWork.Products.Add(item.Product);
            }
            else
            {
                item.Product.ModifiedBy = user.CompanyName;
                item.Product.ModifiedDateTime = DateTime.Now;
                _unitOfWork.Products.Update(item.Product);
            }
            _unitOfWork.SaveChanges();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        return View(item);
    }
    
   /* [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Products.GetAll(includeProperties: "Category");
        return Json(new { data = productList });
    }*/
   
    
    [HttpGet]
    public IActionResult Delete(Guid? id)
    {
        var product = _unitOfWork.Products.GetFirstOrDefault(u=>u.Id==id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
    

// POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id, Product product)
    {
        product.CreatedDateTime = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        if (!ModelState.IsValid)
        {
            _unitOfWork.Products.Remove(product);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(product);
    }

    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        var product = _unitOfWork.Products.GetFirstOrDefault(u => u.Id == id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

}
