using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Dtos
{
    public class EditApplicationUserDto
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
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Id { get; set; }


    }


}
