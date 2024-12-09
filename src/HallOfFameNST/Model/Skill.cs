using System.ComponentModel.DataAnnotations;

namespace HallOfFameNST.Model
{
    /// <summary>
    /// Класс, описывающий навык сотрудника компании.
    /// </summary>
    public class Skill
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public byte Level { get; set; }

        public long PersonId { get; set; }
    }
}