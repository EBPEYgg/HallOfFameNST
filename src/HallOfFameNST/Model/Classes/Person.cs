using System.ComponentModel.DataAnnotations;

namespace HallOfFameNST.Model.Classes
{
    /// <summary>
    /// Класс, описывающий сотрудника компании.
    /// </summary>
    public class Person
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "DisplayName is required.")]
        [StringLength(100, ErrorMessage = "DisplayName cannot be longer than 100 characters.")]
        public string DisplayName { get; set; } = string.Empty;

        public virtual ICollection<Skill> Skills { get; set; } = [];
    }
}