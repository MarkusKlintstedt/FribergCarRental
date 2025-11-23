using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Classes
{
    public class Car
    {
        [Required]
        public int CarId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = "";
        [MaxLength(50)]
        public string ModelName { get; set; } = "";
        [MaxLength(100)]
        public string DisplayBrandModel => $"{Brand} {ModelName}";
        [MaxLength(1000)]
        public string Description { get; set; } = "";
        [Required]
        [Range(0, int.MaxValue)]
        public int RentPricePerDay { get; set; }
        public virtual List<Image> Images { get; set; } = new List<Image>();
        public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
