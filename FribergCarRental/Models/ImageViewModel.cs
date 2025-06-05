using FribergCarRental.Classes;

namespace FribergCarRental.Models
{
    public class ImageViewModel
    {
        public int ImageId { get; set; }
        public string Path { get; set; } = "";
        public int CarId { get; set; }
        public required Car Car { get; set; }
    }
}
