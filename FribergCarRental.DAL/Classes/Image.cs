using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Classes
{
    public class Image
    {
        public int ImageId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Path { get; set; } = "";
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
