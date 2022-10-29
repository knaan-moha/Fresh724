using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fresh724.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fresh724.Entity.Entities;

public class Product:EntityBase
{
  
    [Required (ErrorMessage = "Title is Required")] 
    [StringLength(50,MinimumLength = 5, ErrorMessage = "Title must be min 5 and max 50 characters")]
    [DisplayName("Title")]
    public string Title { get; set; } = string.Empty;
    
    
    [Required (ErrorMessage = "Description is Required")] 
    [StringLength(50,MinimumLength = 5, ErrorMessage = "Description must be min 5 and max 50 characters")]
    [DisplayName("Description")]
    public string Description { get; set; } = string.Empty;
    
    [Required (ErrorMessage = "Price is Required")]
    [Display(Name = "Price")]
    public double PurchasePrice { get; set; }
    
    [Required (ErrorMessage = "Image is Required")]
    [Display(Name = "Image")]
    public string ImageUrl { get; set; }= string.Empty;
    
    
    
    
    //[Required (ErrorMessage = "Category is Required")]
    public Guid CategoryId { get; set; }
    
    public Category Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
    
    public Company? Company { get; set; }
    
    
    [NotMapped]
    public string? TitleWithIcon
    {
        get
        {
            return this.ImageUrl + " " + this.Title;
        }
    }
    
    
}

