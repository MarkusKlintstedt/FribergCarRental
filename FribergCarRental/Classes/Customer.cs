using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Classes
{
    public class Customer
    {
        [Required]
        public int CustomerId { get; set; }
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
        [MaxLength(15)]
        public string Phone { get; set; } = "";

        //Navigation
        public virtual List<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
