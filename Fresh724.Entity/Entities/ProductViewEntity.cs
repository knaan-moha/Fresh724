using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fresh724.Entity.Entities;

public class ProductViewEntity
{
    public Product Product { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem>? CategoryList { get; set; }
}