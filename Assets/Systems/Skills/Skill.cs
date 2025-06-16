using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Systems.Skills
{
    /// <summary>
    /// 
    /// Each Skill has a Name, a Description and a list (Dictionary) of SkillLevel
    /// Different SkillLevels will have different cost, description, etc 
    /// as can be seen in the SkillLevel class.
    /// The skill also has a maxmimum level and information on which is the current 
    /// level of that one skill.
    /// 
    /// </summary>
    public class Skill
    {
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public Dictionary<Level, SkillLevel> SkillLevels { get; set; }

        public Level SkillMaximumLevel { get; private set; }
        public Level SkillCurrentLevel { get; set; }

        public Skill(string skillName, string skillDescription, Dictionary<Level, SkillLevel> skillLevels)
        {
            this.SkillName = skillName;
            this.SkillDescription = skillDescription;
            this.SkillLevels = skillLevels;

            SkillMaximumLevel = (Level)(SkillLevels.Count - 1);
            SkillCurrentLevel = Level.L0;
        }

        public bool IsNotInMaximumLevel
        {
            get
            {
                return SkillCurrentLevel < SkillMaximumLevel;
            }            
        }

        public void UpgradeSkill()
        {
            if (IsNotInMaximumLevel)
            {
                SkillCurrentLevel += 1;
            }
        }

        public void ResetSkill()
        {
            SkillCurrentLevel = Level.L0;
        }
    }
}
