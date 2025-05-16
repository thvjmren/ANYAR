using System.ComponentModel.DataAnnotations;

namespace Anyar.ViewModels
{
    public class GetEmployeeVM
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "only letters can be used")]
        public string Name { get; set; }
        public int Id {  get; set; }
        public string Image { get; set; }
        public string Instagram { get; set; }
        public string X { get; set; }
        public string Facebook { get; set; }
        public string Linkedin { get; set; }
        public string PositionName { get; set; }
    }
}
