using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Fresh724.Entity.Entities;

public class ApplicationUser:IdentityUser
{
    [Display(Name = "CompanyName")]
    public string CompanyName{ get; set; }=string.Empty;
    
    [Display(Name = "CompanyId")]
    public Guid CompanyId{ get; set; }
}