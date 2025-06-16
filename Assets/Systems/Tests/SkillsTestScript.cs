using System.Collections;
using System.Collections.Generic;
using Assets.Systems.Skills;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class SkillsTestScripts
{
    private SkillsList skillsList;
    private Dictionary<Level, SkillLevel> defaultSkillLevels;
    private Dictionary<Level, SkillLevel> limitedSkillLevels;

    [SetUp]
    public void Setup()
    {        
        defaultSkillLevels = new Dictionary<Level, SkillLevel>
        {
            { Level.L0, new SkillLevel(Level.L0, "Beginner Attack", "Basic attack", 0) },
            { Level.L1, new SkillLevel(Level.L1, "Intermediate Attack", "Stronger attack", 5) },
            { Level.L2, new SkillLevel(Level.L2, "Advanced Attack", "Powerful attack", 10) }
        };

        limitedSkillLevels = new Dictionary<Level, SkillLevel>
        {
            { Level.L0, new SkillLevel(Level.L0, "Beginner Attack", "Basic attack", 0) },
            { Level.L1, new SkillLevel(Level.L1, "Intermediate Attack", "Stronger attack", 5) }            
        };

        Dictionary<string, Skill> skills = new Dictionary<string, Skill>
        {
            { "Jump", new Skill("Jump", "Increases jump height", new Dictionary<Level, SkillLevel>
                {
                    { Level.L0, new SkillLevel(Level.L0, "Basic Jump", "Normal jump height", 0) },
                    { Level.L1, new SkillLevel(Level.L1, "High Jump", "Improved jump height", 5) },
                    { Level.L2, new SkillLevel(Level.L2, "Ultra Jump", "Maximum jump height", 10) }
                })
            },
            { "Attack", new Skill("Attack", "Increases attack power", new Dictionary<Level, SkillLevel>
                {
                    { Level.L0, new SkillLevel(Level.L0, "Weak Attack", "Standard damage", 0) },
                    { Level.L1, new SkillLevel(Level.L1, "Strong Attack", "Enhanced damage", 5) },
                    { Level.L2, new SkillLevel(Level.L2, "Power Attack", "Maximum damage", 10) }
                })
            },
            { "Heal", new Skill("Heal", "Restores player health", new Dictionary<Level, SkillLevel>
                {
                    { Level.L0, new SkillLevel(Level.L0, "Small Heal", "Minor health recovery", 0) },
                    { Level.L1, new SkillLevel(Level.L1, "Medium Heal", "Moderate health recovery", 5) },
                    { Level.L2, new SkillLevel(Level.L2, "Full Heal", "Maximum health recovery", 10) }
                })
            }
        };
        skillsList = new SkillsList(skills);
    }

    [Test]
    public void Skill_CurrentLevelIsZeroWhenInitialized()
    {        
        // ARRANGE - Done in the Setup

        // ACT
        Skill skill = new Skill("Attack", "Default attack", defaultSkillLevels);

        // ASSERT
        Assert.AreEqual(Level.L0, skill.SkillCurrentLevel);        
    }

    [Test]
    public void Skill_MaximumSkillLevelDependsOnDefinedSkillLevels()
    {
        // ARRANGE - Done in the Setup

        // ACT
        Skill defaultSkill = new Skill("Attack", "Default attack", defaultSkillLevels);
        Skill limitedSkill = new Skill("WeakAttack", "Weaker attack", limitedSkillLevels);

        // ASSERT
        Assert.AreEqual(Level.L2, defaultSkill.SkillMaximumLevel);
        Assert.AreEqual(Level.L1, limitedSkill.SkillMaximumLevel);
    }

    [Test]
    public void Skill_UpgradeWorks()
    {
        // ARRANGE
        Skill skill = new Skill("Attack", "How good is the attack", defaultSkillLevels);

        // ACT
        skill.UpgradeSkill();

        // ASSERT
        Assert.AreEqual(Level.L1, skill.SkillCurrentLevel);
    }

    [Test]
    public void Skill_CannotGoAboveMaxLevel()
    {
        // ARRANGE
        Skill skill = new Skill("Attack", "How good is the attack", defaultSkillLevels); // Maximum Level L2

        // ACT
        skill.UpgradeSkill();
        skill.UpgradeSkill();
        skill.UpgradeSkill();
        skill.UpgradeSkill();
        skill.UpgradeSkill();

        // ASSERT
        Assert.AreEqual(Level.L2, skill.SkillCurrentLevel);
    }

    [Test]
    public void SkillsList_GetTheCorrectSkillLevel()
    {
        // ARRANGE
        skillsList.Skills["Jump"].UpgradeSkill(); // Upgrade to L1
        skillsList.Skills["Attack"].UpgradeSkill();
        skillsList.Skills["Attack"].UpgradeSkill(); // Upgrade to L2
        // Heal remains in L0

        // ACT
        Level jumpLevel = skillsList.GetLevelOfSkill("Jump");
        Level attackLevel = skillsList.GetLevelOfSkill("Attack");
        Level healLevel = skillsList.GetLevelOfSkill("Heal");

        // ASSERT
        Assert.AreEqual(Level.L1, jumpLevel);
        Assert.AreEqual(Level.L2, attackLevel);
        Assert.AreEqual(Level.L0, healLevel);
    }

    [Test]
    public void SkillsList_ResetSkillsProperly()
    {
        // ARRANGE - Upgrade all Skills        
        skillsList.Skills["Jump"].UpgradeSkill();
        skillsList.Skills["Attack"].UpgradeSkill();
        skillsList.Skills["Heal"].UpgradeSkill();

        // ACT
        skillsList.ResetAllSkills();
        Level jumpLevel = skillsList.GetLevelOfSkill("Jump");
        Level attackLevel = skillsList.GetLevelOfSkill("Attack");
        Level healLevel = skillsList.GetLevelOfSkill("Heal");

        // ASSERT
        Assert.AreEqual(Level.L0, jumpLevel);
        Assert.AreEqual(Level.L0, attackLevel);
        Assert.AreEqual(Level.L0, healLevel);
    }
}