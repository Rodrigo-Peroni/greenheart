namespace Assets.Utils.Skills
{
    /// <summary>
    /// This class persists the information regardin a single Skill Level
    /// </summary>
    public class SkillLevel
    {
        public Level Level { get; set; }
        public string SkillLevelName { get; set; }
        public string SkillLevelDescrition { get; set; }
        public int SkillLevelCost { get; set; }

        public SkillLevel(Level level, string skillLevelName, string skillLevelDescription, int skillLevelCost)
        {
            this.Level = level;
            this.SkillLevelName = skillLevelName;
            this.SkillLevelDescrition = skillLevelDescription;
            this.SkillLevelCost = skillLevelCost;
        }
    }
}
