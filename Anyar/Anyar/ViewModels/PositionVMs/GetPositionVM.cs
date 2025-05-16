using Anyar.Models;
using System.ComponentModel.DataAnnotations;

namespace Anyar.ViewModels
{
    public class GetPositionVM
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "only letters can be used")]
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
