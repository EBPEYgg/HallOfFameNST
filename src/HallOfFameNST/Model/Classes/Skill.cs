using System.ComponentModel.DataAnnotations;

namespace HallOfFameNST.Model.Classes
{
    public class Skill
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Level is required.")]
        [Range(1, 10, ErrorMessage = "Level must be between 1 and 10.")]
        public byte Level { get; set; }

        [Required(ErrorMessage = "PersonId is required.")]
        public long PersonId { get; set; }
    }
}