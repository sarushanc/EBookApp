using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EBookApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public required string Title { get; set; }
        public string? Author { get; set; }

        [Precision(18, 2)]
        public required decimal Price { get; set; }

        public string? Category { get; set; }
        public string? ISBN { get; set; }
    }
}
