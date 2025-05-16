using Anyar.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Anyar.Models
{
    public class Position:BaseEntity
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "only letters can be used")]
        public string Name { get; set; }
        public List<Employee>? Employees { get; set; }
    }
}
