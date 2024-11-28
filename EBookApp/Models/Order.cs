using System.ComponentModel.DataAnnotations;

namespace EBookApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public int BillingDetailsId { get; set; }
        public CheckoutDetails? BillingDetails { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
