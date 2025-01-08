namespace EBookApp.Models
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<AdminOrderItemViewModel> Items { get; set; } = new List<AdminOrderItemViewModel>();
    }

    public class AdminOrderItemViewModel
    {
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
