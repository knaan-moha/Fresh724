using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fresh724.Core.Entities;

namespace Fresh724.Entity.Entities;

public class Company:EntityBase
{
    [Required (ErrorMessage = "Name is Required")]
    [StringLength(50)]
    [DisplayName("CompanyName")]
    public string Name { get; set; } = string.Empty;
    
   // [Required (ErrorMessage = "Icon is Required")]
    [Display(Name = "Icon")]
    public string IconUrl { get; set; }= string.Empty;
    
}




