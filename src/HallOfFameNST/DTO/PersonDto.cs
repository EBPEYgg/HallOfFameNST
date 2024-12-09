using System.ComponentModel.DataAnnotations;

namespace HallOfFameNST.DTO
{
    public class PersonDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "DisplayName is required.")]
        [StringLength(100, ErrorMessage = "DisplayName cannot be longer than 100 characters.")]
        public string DisplayName { get; set; } = string.Empty;

        public IEnumerable<SkillDto> Skills { get; set; } = [];
    }
}