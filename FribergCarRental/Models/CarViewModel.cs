namespace FribergCarRental.Models
{
    public class CarViewModel
    {
        public int CarId { get; set; }
        public string Brand { get; set; } = "";
        public string ModelName { get; set; } = "";
        public string Description { get; set; } = "";
        public int RentPricePerDay { get; set; }
    }
}
