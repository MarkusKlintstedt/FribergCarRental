using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Models
{
    public class CarViewModel
    {
        [Required]
        public int CarId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = "";
        [MaxLength(1000)]
        public string Description { get; set; } = "";
        [Required]
        [Range(0, int.MaxValue)]
        public int RentPricePerDay { get; set; }
    }
}
