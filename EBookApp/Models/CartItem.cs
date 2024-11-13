using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EBookApp.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Foreign key for Book
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book? Book { get; set; }  // Navigation property - not required

        // Foreign key for User
        public required string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; } // Navigation property - not required

        public int Quantity { get; set; }
    }
}
