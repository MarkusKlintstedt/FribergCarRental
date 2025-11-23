using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Dtos
{
    public class ApplicationUserDto
    {
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
        [EmailAddress]
        public string Email { get; set; }
        public string Id { get; set; }
        //public string? PhoneNumber { get; set; }
        //public string? test { get; set; }



    }
}
