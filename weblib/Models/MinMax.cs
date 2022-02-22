using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace weblib.Models
{
    public class MinMax : IValidatableObject
    {
        [Required]
        [Range(0, 10)]
        public int? Min { get; set; } // When not nullable, the default value (0) will make the Required-attribute not do anything

        [Required]
        [Range(0, 10)]
        public int? Max { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Min > Max)
            {
                yield return new ValidationResult("Min cannot be more than Max", new string[] { nameof(Min), nameof(Max) });    
            }
        }
    }
}
