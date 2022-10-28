using System.ComponentModel.DataAnnotations;
using Fresh724.Core.Entities;

namespace Fresh724.Entity.Entities;

public class OrderShopping:AddressBase
{

    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentType { get; set; }
    public Product Product { get; set; }
    public Guid ProductId { get; set; }
    public virtual ApplicationUser User { get; set; }
    public string PhoneNumber { get; set; }
    public string UserId { get; set; }
}