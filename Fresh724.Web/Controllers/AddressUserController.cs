using Fresh724.Data.Context;
using Fresh724.Data.Repository.Abstract;
using Fresh724.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Fresh724.Web.Controllers;

public class AddressUserController : Controller
{
    // GET
    private readonly ILogger<AddressUserController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _um;

    public AddressUserController(ILogger<AddressUserController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationUser> um)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _um = um;
    }

    [AllowAnonymous]
  
   
        
     /*   public async Task<IActionResult> Index(string SearchString)
        {
            ViewData["CurrentFilter"] = SearchString;
            var address2 = from m in _unitOfWork.AddressUsers.GetAll()
                select m;
               
            
             if (!String.IsNullOrEmpty(SearchString))
            {
                address2 = address2.Where(s => s.Street1!.Contains(SearchString));
            }
            
             
            return View(await address2.ToListAsync());
            
            
            // var user = _um.GetUserAsync(User).Result;
            //IEnumerable<AddressUser> address = _unitOfWork.AddressUsers.GetByFilter( x => x.UserId == user.Id);
            //return View(address);
            
        }*/
     
     
     public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
     {
         ViewBag.CurrentSort = sortOrder;
         ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "street1_desc" : "";
         ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "city_desc" : "";
         ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "state_desc" : "";
         ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "zipCode_desc" : "";
         ViewBag.DateSortParm = sortOrder == "ZipCode" ? "zipCode_desc" : "ZipCode";

         if (searchString != null)
         {
             page = 1;
         }
         else
         {
             searchString = currentFilter;
         }

         ViewBag.CurrentFilter = searchString;

         var address = from s in _unitOfWork.AddressUsers.GetAll()
             select s;
         if (!string.IsNullOrEmpty(searchString))
         {
             address = address.Where(s => s.Street1.Contains(searchString)
                                            || s.City.Contains(searchString)
                                            || s.State.Contains(searchString)
                                            || s.ZipCode.Contains(searchString));
         }

         switch (sortOrder)
         {
             case "street1_desc":
                 address = address.OrderByDescending(s => s.Street1);
                 break;
             case "Zipcode_desc":
                 address = address.OrderByDescending(s => s.ZipCode);
                 break;
             case "city_desc":
                 address = address.OrderByDescending(s => s.City);
                 break;
             
             case "state_desc":
                 address = address.OrderByDescending(s => s.State);
                 break;
             case "ZipCode":
                 address = address.OrderBy(s => s.ZipCode);
                 break;
             case "zipCode_desc":
                 address = address.OrderByDescending(s => s.ZipCode);
                 break;
             default: // Name ascending 
                 address = address.OrderBy(s => s.Street1);
                 break;

         }

         int pageSize = 2;
         int pageNumber = (page ?? 1);
         return View(address.ToPagedList(pageNumber, pageSize));
     }


   

    [Authorize]
    [HttpGet]
    public IActionResult  Add()
    {
        var user = _um.GetUserAsync(User).Result;
        
        AddressUser address = new();
        address.User = user;
        address.UserId = user.Id;
        IEnumerable<AddressUser> address2 = _unitOfWork.AddressUsers.GetByFilter( x => x.UserId == user.Id);
        if(address == null || address2 == null)
        {
            return View();
        }
        
            
        
        return View(address);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add([Bind("Street1, Street2, City, State,ZipCode,Country, User, UserId") ]AddressUser addressUser)
    {
        
        var user = _um.GetUserAsync(User).Result;
        addressUser.User = user;
        addressUser.UserId = user.Id;


        if (ModelState.IsValid)
        {
            _unitOfWork.AddressUsers.Add(addressUser);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Address Added successfully";
            return RedirectToAction("Index");
        }
        else
        {
            return View(addressUser);
        }
        
    }



    [Authorize]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if(id==null || id == Guid.Empty)
        {
            return NotFound();
        }
        
        var addressUser = _unitOfWork.AddressUsers.GetFirstOrDefault(u=>u.Id==id);
        
        if (addressUser == null)
        {
            return NotFound();
        }

        return View(addressUser);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AddressUser addressUser)
    {
        var user = _um.GetUserAsync(User).Result;
        addressUser.User = user;
        addressUser.UserId = user.Id;


        if (ModelState.IsValid)
        {
            _unitOfWork.AddressUsers.Update(addressUser);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Address edited successfully";
            return RedirectToAction("Index");
        }

        return View(addressUser);
    }
    
    // GET: Address/Delete/5
    public IActionResult Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var address =  _unitOfWork.AddressUsers.GetFirstOrDefault(m => m.Id == id);
        
        if (address == null)
        {
            return NotFound();
        }

        return View(address);
    }
    
    // POST: Address/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var address =  _unitOfWork.AddressUsers.GetFirstOrDefault(u => u.Id == id);;
        _unitOfWork.AddressUsers.Remove(address);
        _unitOfWork.SaveChanges();
        TempData["success"] = "Address deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}