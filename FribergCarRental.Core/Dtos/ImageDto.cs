using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Dtos
{
    public class ImageDto
    {
        public int ImageId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Path { get; set; } = "";
        public int CarId { get; set; }
    }
}
