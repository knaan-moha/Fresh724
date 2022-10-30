using System.ComponentModel.DataAnnotations.Schema;

namespace Fresh724.Entity.Entities;

public class ShoppingCart
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    [ForeignKey("ProductId")]

    public Product Product { get; set; }
    public int Count { get; set; }

    public string UserId { get; set; }
    [ForeignKey("ApplicationUserId")]

    public ApplicationUser User { get; set; }

    [NotMapped]
    public double Price { get; set; }
}
