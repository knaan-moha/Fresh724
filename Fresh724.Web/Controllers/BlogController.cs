using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Fresh724.Data.Context;
using Fresh724.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Fresh724.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fresh724.Web.Controllers;

public class BlogController : Controller
{
    private readonly ILogger<BlogController> _logger;
    private readonly ApplicationDbContext _unitOfWork;
    private readonly UserManager<ApplicationUser> _um;

    public BlogController(ILogger<BlogController> logger, ApplicationDbContext unitOfWork, UserManager<ApplicationUser> um)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _um = um;
    }
    
    [AllowAnonymous]
    public IActionResult Index()
    {
        var objPostList = _unitOfWork.Posts.OrderByDescending(b=>b.Id).ToList();
        
        
        var post =  _unitOfWork.Posts
            .Include(blog => blog.User)
            .ToListAsync();
        return View(objPostList);
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("Title, Summary, Content, User, UserId") ]Posts posts)
    {
        posts.Timestamp = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        posts.User = user;
        posts.UserId = user.Id ;
        
        
        if (!ModelState.IsValid)
        {
            _unitOfWork.Posts.Add(posts);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(posts);
    }
    
   //Get Edit

   [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = _um.GetUserAsync(User).Result;
      
       
        //var posts = await _db.Posts.FirstOrDefaultAsync(m=>m.Id==id);
        var posts = await _unitOfWork.Posts.FindAsync(id);
        if (posts == null || posts.UserId !=user.Id )
        {
            return RedirectToAction(nameof(Index));
        }
        return View(posts);
    }
    
   
 
   
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Edit(Guid id,[Bind("Title, Summary, Content, User, UserId") ]Posts posts)
   {
       posts.Timestamp = DateTime.Now;
       var user = _um.GetUserAsync(User).Result;
       
       posts.User = user;
       posts.UserId = user.Id ;
        
        
       if (!ModelState.IsValid)
       {
           _unitOfWork.Posts.Update(posts);
           _unitOfWork.SaveChanges();
           return RedirectToAction("Index");
       }

       return View(posts);
   }
 /*  public async Task<IActionResult> Edit([Bind("Id,Title, Summary, Content, user, userId") ]Posts posts)
    {
        
        posts.Timestamp = DateTime.Now;
        var user = _um.GetUserAsync(User).Result;
        posts.User = user;
        posts.UserId = user.Id ;
        
            if (!ModelState.IsValid )
            {

                try
                {
                    _db.Update(posts);
                    _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("Error");
                    throw;
                }

                return RedirectToAction("Index");
            }

            return View(posts);

    }
   
   */
   
  

}