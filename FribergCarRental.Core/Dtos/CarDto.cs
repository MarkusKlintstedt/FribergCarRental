using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Dtos
{
    public class CarDto
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
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}
