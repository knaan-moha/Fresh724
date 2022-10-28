using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fresh724.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Fresh724.Entity.Entities
{
    public class Employee:EntityBase
    {
        
        
        [Required]
        [Display(Name = "First Name")]
        [Column(TypeName = "nvarchar(15)")]
        public string FirstName { get; set; }=string.Empty;
        
        [Required]
        [Display(Name = "Last Name")]
        [Column(TypeName = "nvarchar(15)")]
        public string LastName { get; set; }=string.Empty;
        
        public string CompanyName { get; set; }=String.Empty;
    
        //[ForeignKey("CompanyId")]
        //public Company Company { get; set; }
        
     
        [Display(Name = "ImageUrl")]
        [ValidateNever]
        public string ImageUrl { get; set; }
        
    }
}
