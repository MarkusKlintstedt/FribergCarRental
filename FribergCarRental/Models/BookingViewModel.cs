using System.ComponentModel.DataAnnotations;
using FribergCarRental.Classes;

namespace FribergCarRental.Models
{
    public class BookingViewModel : IValidatableObject
    {
        public int BookingId { get; set; }
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        //public string UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RentStartDate.Day < DateTime.Today.Day)
            {
                yield return new ValidationResult("Start day must be in future", new[] { nameof(RentStartDate) });
            }
            if (RentEndDate.Day < RentStartDate.Day)
            {
                yield return new ValidationResult("End date must be after start date", new[] { nameof(RentEndDate) });
            }

        }
    }
}
