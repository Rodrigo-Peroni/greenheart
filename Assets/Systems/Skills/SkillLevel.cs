namespace Assets.Systems.Skills
{
    /// <summary>
    /// 
    /// The SkillLevel class persists the information regarding a single Skill Level
    /// It contains a name, a description and a cost to upgrade to the next level.
    /// 
    /// The changes in the character behavior that a given Skill level provides
    /// are not implemented here, but in the Player class, in the methods LoadJumpSkill()
    /// LoadAttackSkill() and LoadHealSkill(). These method can be found in the 
    /// region "Skill Loading Methods".
    /// 
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
