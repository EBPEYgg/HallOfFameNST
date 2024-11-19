using System.ComponentModel.DataAnnotations;

namespace HallOfFameNST.Model.Classes
{
    public class Skill
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [Range(1, 10, ErrorMessage = "Level must be between 1 and 10.")]
        public byte Level { get; set; }

        public long PersonId { get; set; }
    }
}