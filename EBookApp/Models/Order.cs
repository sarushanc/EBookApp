namespace EBookApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int BillingDetailsId { get; set; }
        public CheckoutDetails? BillingDetails { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
