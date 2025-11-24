using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Models
{
    public class UserViewModel
    {
        public String? Id { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [MaxLength(50)]
        public string LastName { get; set; } = "";
        [MaxLength(100)]
        public string Address { get; set; } = "";
        [MaxLength(50)]
        public string City { get; set; } = "";
        [MaxLength(10)]
        public string ZipCode { get; set; } = "";
    }
}
