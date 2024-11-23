using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EBookApp.Models
{
    public class CheckoutDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BillingAddress { get; set; } = string.Empty;

        [Required]
        public string BillingCity { get; set; } = string.Empty;

        [Required]
        public string BillingPostalCode { get; set; } = string.Empty;

        [Required]
        public string BillingCountry { get; set; } = string.Empty;

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }
    }
}
