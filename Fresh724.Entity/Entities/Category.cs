using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fresh724.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fresh724.Entity.Entities;

public class Category:EntityBase
{
    
    
    [Required (ErrorMessage = "Name is Required")]
    [StringLength(50)]
    [DisplayName("Name")]
    public string Name { get; set; } = string.Empty;
    
    //public string UserId { get; set; }= string.Empty;
    //public ApplicationUser User { get; set; }

    public string ImageUrl { get; set; }= string.Empty;
    
    
    [NotMapped]
    public string? TitleWithIcon
    {
        get
        {
            return this.ImageUrl + " " + this.Name;
        }
    }
    
}


