using Assets.Systems.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsList : MonoBehaviour
{
    [Serializable]
    public struct SkillEntry
    {
        public string skillName;
        public string skillDescription;
        public SkillLevelEntry[] skillLevelEntries;
    }

    [Serializable]
    public struct SkillLevelEntry
    {
        public Level skillLevel;
        public string skillLevelName;
        public string skillLevelDescrition;
        public int skillLevelCost;
    }    

    public List<SkillEntry> skillEntries;

    public Dictionary<string, Skill> Skills { get; private set; }

    /// <summary>
    /// Constructor created exclusively for testing purposes.
    /// </summary>
    /// <param name="skills">the Skill list</param>
    public SkillsList(Dictionary<string, Skill> skills)
    {
        this.Skills = skills;
    }

    /// <summary>
    /// The Skills Dictionary is loaded on the Awake() method because 
    /// loading a Dictionary directly from the Unity Editor was not 
    /// available in the Unity version where it was developed.
    /// </summary>
    void Awake()
    {
        Skills = new Dictionary<string, Skill>();

        foreach(SkillEntry entry in skillEntries)
        {
            Dictionary<Level, SkillLevel>  skillLevels = new Dictionary<Level, SkillLevel>();

            foreach(SkillLevelEntry skillLevel in entry.skillLevelEntries)
            {
                skillLevels.Add(
                    skillLevel.skillLevel,
                    new SkillLevel(
                        skillLevel.skillLevel, 
                        skillLevel.skillLevelName, 
                        skillLevel.skillLevelDescrition, 
                        skillLevel.skillLevelCost)
                    );
            }

            Skills.Add(entry.skillName, new Skill(entry.skillName, entry.skillDescription, skillLevels));
        }
    }

    public Level GetLevelOfSkill(string skillName)
    {
        if (Skills.TryGetValue(skillName, out Skill skill))
        {
            return skill.SkillCurrentLevel;
        }
        else
        {
            return Level.L0;
        }
    }

    public void ResetAllSkills()
    {
        foreach(KeyValuePair<string, Skill> entry in Skills)
        {
            entry.Value.ResetSkill();
        }
    }
}
