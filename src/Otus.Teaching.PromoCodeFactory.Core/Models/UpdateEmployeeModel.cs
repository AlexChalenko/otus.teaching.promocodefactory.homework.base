using System.ComponentModel.DataAnnotations;

namespace Otus.Teaching.PromoCodeFactory.Core.Models
{
    public class UpdateEmployeeModel
    {

        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}
