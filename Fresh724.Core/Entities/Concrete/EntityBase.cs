using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fresh724.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class EntityBase:IEntityBase
{
    [Key]
    public Guid Id { get; set; }
    
   
    public  string Status { get; set; }= string.Empty;
    public  string CreatedBy { get; set; }= string.Empty;
    public string? ModifiedBy { get; set; }= string.Empty;
    public  DateTime CreatedDateTime { get; set; } = DateTime.Now;
    
    [DefaultValue(" ")]
    public  DateTime? ModifiedDateTime { get; set; }
}