namespace EBookApp.Models
{
    public class CheckoutViewModel
    {
        public int? SelectedCheckoutId { get; set; } // To select existing details
        public List<CheckoutDetails> SavedDetails { get; set; } = new();
        public CheckoutDetails NewDetails { get; set; } = new();
    }
}
