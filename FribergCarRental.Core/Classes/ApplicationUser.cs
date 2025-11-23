using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FribergCarRental.Core.Classes
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";
        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string City { get; set; } = "";
        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = "";
        public virtual List<Booking> Bookings { get; set; } = new() { };
    }
}
