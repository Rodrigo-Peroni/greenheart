using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Utils.Skills
{
    public class Skill
    {
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public Dictionary<Level, SkillLevel> SkillLevels { get; set; }

        private readonly Level skillMaximumLevel;
        public Level SkillCurrentLevel { get; set; }

        public Skill(string skillName, string skillDescription, Dictionary<Level, SkillLevel> skillLevels)
        {
            this.SkillName = skillName;
            this.SkillDescription = skillDescription;
            this.SkillLevels = skillLevels;

            skillMaximumLevel = (Level)(SkillLevels.Count - 1);
            SkillCurrentLevel = Level.L0;
        }

        public bool IsNotInMaximumLevel
        {
            get
            {
                return SkillCurrentLevel < skillMaximumLevel;
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
