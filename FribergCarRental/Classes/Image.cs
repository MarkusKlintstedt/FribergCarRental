using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Classes
{
    public class Image
    {
        [Required]
        public int ImageId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Path { get; set; } = "";
        public required Car Car { get; set; }
    }
}
