namespace HallOfFameNST.DTO
{
    public class PersonDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<SkillDto> Skills { get; set; } = [];
    }
}