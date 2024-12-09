namespace HallOfFameNST.Model
{
    /// <summary>
    /// Класс, описывающий сотрудника компании.
    /// </summary>
    public class Person
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public virtual ICollection<Skill> Skills { get; set; } = [];
    }
}