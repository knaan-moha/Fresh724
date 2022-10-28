using Fresh724.Data.Context;
using Fresh724.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fresh724.Web.Controllers;

 [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly ApplicationDbContext _unitofWork;

        public CategoryApiController(ApplicationDbContext unitofWork)
        {
            _unitofWork = unitofWork;
        }

        // GET: api/CategoryApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _unitofWork.Categories.ToListAsync();
        }

        // GET: api/CategoryApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(Guid id)
        {
            var category = await _unitofWork.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/GuestbookApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(Guid id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _unitofWork.Entry(category).State = EntityState.Modified;

            try
            {
                await _unitofWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GuestbookApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _unitofWork.Categories.Add(category);
            await _unitofWork.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/GuestbookApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var guestbook = await _unitofWork.Categories.FindAsync(id);
            if (guestbook == null)
            {
                return NotFound();
            }

            _unitofWork.Categories.Remove(guestbook);
            await _unitofWork.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(Guid id)
        {
            return _unitofWork.Categories.Any(e => e.Id == id);
        }
    }
