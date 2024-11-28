namespace EBookApp.Models
{
    public class UserOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<UserOrderItemViewModel> Items { get; set; } = new List<UserOrderItemViewModel>();
    }

    public class UserOrderItemViewModel
    {
        public string BookTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
