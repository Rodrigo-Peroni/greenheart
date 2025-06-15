using Assets.Systems.Skills;
using UnityEngine;
using UnityEngine.UI;

public class SkillsMenu : MonoBehaviour
{
    public float commandRate;

    public Image handSelector;
    public Sprite redSelectorImage;
    public Transform healthSkillHandPoint;
    public Transform attackSkillHandPoint;
    public Transform jumpSkillHandPoint;
    public Transform doneButtonHandPoint;
    public GameObject selectorFrame;
    public GameObject doneButtonSelectorFrame;
    public GameObject healthSkillBar;
    public GameObject attackSkillBar;
    public GameObject jumpSkillBar;
    public Text diamondValueText;

    public AudioClip menuChangeSfx;
    public AudioClip menuConfirmSfx;
    public AudioClip menuSuccessSfx;
    public AudioClip menuFailureSfx;

    private float nextCommand;
    private int handPosition = 1;

    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        doneButtonSelectorFrame.SetActive(false);
        MoveCursorToPosition(handPosition, true);

        LoadDiamondValue();
        LoadValuesForSkill("Heal");
        LoadValuesForSkill("Attack");
        LoadValuesForSkill("Jump");
    }

    private void LoadDiamondValue()
    {
        diamondValueText.text = "x" + playerManager.diamondsAmount;
    }

    /// <summary>
    /// Shows in the skill menu the name, description and cost of the skills' next level.
    /// </summary>
    /// <param name="skillName">Name of the skill</param>
    private void LoadValuesForSkill(string skillName)
    {        
        Skill skill = playerManager.SkillsList.Skills[skillName];
        Level currentLevel = skill.SkillCurrentLevel;
        SkillLevel skillCurrentLevel = skill.SkillLevels[currentLevel];

        GameObject skillBar = GetSkillBarForSkill(skillName);

        Text skillNameText = skillBar.transform.Find("UI_SkillName_Text").GetComponent<Text>();
        Text skillDescriptionText = skillBar.transform.Find("UI_SkillDescription_Text").GetComponent<Text>();
        GameObject costObject = skillBar.transform.Find("UI_SkillCost_Text").gameObject;
        Text skillCostText = costObject.GetComponent<Text>();
        GameObject diamondIcon = costObject.transform.Find("UI_DiamondIcon").gameObject;

        if (skill.IsNotInMaximumLevel)
        {
            // Shows name, description and cost for the next SkillLevel
            SkillLevel skillNextLevel = skill.SkillLevels[currentLevel+1];

            skillNameText.text = skillNextLevel.SkillLevelName;
            skillDescriptionText.text = skillNextLevel.SkillLevelDescrition;
            skillCostText.text = "Cost:  x" + skillNextLevel.SkillLevelCost;
            diamondIcon.SetActive(true);
        }
        else
        {
            // Shows only information htat the skill level is maximized
            skillNameText.text = "";
            skillDescriptionText.text = "Maximum Level";
            skillDescriptionText.fontSize = 20;
            skillCostText.text = "";
            diamondIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the GameObject referring to the given skill (in the Scene SkillMenu)
    /// </summary>
    /// <param name="skillName">Name of the skill</param>
    /// <returns>GameObject that containd the skill bar for the given skill</returns>
    private GameObject GetSkillBarForSkill(string skillName)
    {
        switch (skillName)
        {
            case "Heal":
                return healthSkillBar;
            case "Attack":
                return attackSkillBar;
            case "Jump":
                return jumpSkillBar;
            default:
                return null;
        }
    }

    /// <summary>
    /// The Update method is being used to get the keyboard/controller inputs
    /// and move the cursor through the available skills.
    /// 
    /// Detailed implementations can be found in the methods in regions
    /// "Cursor Movement" and "Get Directional Value"
    /// </summary>
    void Update()
    {
        float horizontalValue = GetHorizontalValue();
        float verticalValue = GetVerticalValue();

        if (horizontalValue != 0f && Time.time > nextCommand)
        {
            if (horizontalValue < 0)
            {
                MoveCursorLeft();
            }
            else
            {
                MoveCursorRight();
            }
            nextCommand = Time.time + commandRate;
        }

        if (verticalValue !=0f && Time.time > nextCommand)
        {
            if (verticalValue < 0)
            {
                MoveCursorDown();
            }
            else
            {
                MoveCursorUp();
            }
            nextCommand = Time.time + commandRate;
        }

        if (Input.GetButtonDown("Submit") && Time.time > nextCommand)
        {
            PerformActionOnCurrentSelection();
        }
    }

    /// <summary>
    /// Performs the action corresponding to the selected button, which can be upgrade a skill
    /// or clicking on the "Done" button.
    /// 
    /// The verification of which is the current button selected is done by verifying the value of handPosition (called "hand" because 
    /// the cursor is depicted with a hand image).
    /// The manipulation of the handPosition value can be found in the methods located in the region "Cursor Movement".
    /// </summary>
    private void PerformActionOnCurrentSelection()
    {
        if (handPosition == 0)
        {
            SoundManager.instance.PlaySound(menuConfirmSfx);
            GoToNextLevel();
        }
        else
        {
            Skill currentSkill = GetSelectedSkill();
            if (CanCurrentSkillBeUpgraded())
            {
                currentSkill.UpgradeSkill();
                SoundManager.instance.PlaySound(menuSuccessSfx);
                playerManager.diamondsAmount -= currentSkill.SkillLevels[currentSkill.SkillCurrentLevel].SkillLevelCost;
                MoveCursorToPosition(handPosition, true);
                RefreshCurrentSkillBar();
                LoadDiamondValue();
            }
            else
            {
                SoundManager.instance.PlaySound(menuFailureSfx);
            }
        }
    }

    /// <summary>
    /// Go to next game level according to the PlayerManager
    /// </summary>
    private void GoToNextLevel()
    {
        Hud.instance.TurnOffBlur();
        Hud.instance.HideSkillsMenu();
        playerManager.LoadNextLevel();
    }

    /// <summary>
    /// Used to refresh the values of a single skill bar once that skill is upgraded.
    /// </summary>
    private void RefreshCurrentSkillBar()
    {
        switch (handPosition)
        {
            case 1:
                LoadValuesForSkill("Heal");
                break;
            case 2:
                LoadValuesForSkill("Attack");
                break;
            case 3:
                LoadValuesForSkill("Jump");
                break;
        }
    }

    /// <summary>
    /// Returns if a skill can be upgraded based on the information of the Skill maximum level.
    /// </summary>
    /// <returns>Returns true if the skill can be upgraded, false otherwise</returns>
    private bool CanCurrentSkillBeUpgraded()
    {
        Skill currentSkill = GetSelectedSkill();
        if (currentSkill.IsNotInMaximumLevel)
        {
            if (playerManager.diamondsAmount >= currentSkill.SkillLevels[currentSkill.SkillCurrentLevel+1].SkillLevelCost)
            {
                return true;
            }
        }                
        return false;        
    }

    /// <summary>
    /// Get the selected skill based on the handPosition value.
    /// The manipulation of the handPosition value cna be found in the methods located in the region "Cursor Movement".
    /// </summary>
    /// <returns>Returns the current selected Skill</returns>
    private Skill GetSelectedSkill()
    {        
        switch (handPosition)
        {
            case 1:
                return playerManager.SkillsList.Skills["Heal"];                
            case 2:
                return playerManager.SkillsList.Skills["Attack"];                
            case 3:
                return playerManager.SkillsList.Skills["Jump"];                
            default:
                return null;
        }
    }

    #region Cursor Movement

    private void MoveCursorLeft()
    {
        if (handPosition == 0)
        {
            handPosition = 1;
        }
        else if (handPosition == 1)
        {
            handPosition = 3;
        }
        else
        {
            handPosition--;
        }
        MoveCursorToPosition(handPosition);
    }

    private void MoveCursorRight()
    {
        if (handPosition == 0)
        {
            handPosition = 3;
        }
        else if (handPosition == 3)
        {
            handPosition = 1;
        }
        else
        {
            handPosition++;
        }
        MoveCursorToPosition(handPosition);
    }

    private void MoveCursorDown()
    {
        if (handPosition != 0)
        {
            handPosition = 0;
            MoveCursorToPosition(handPosition);
        }
    }

    private void MoveCursorUp()
    {
        if (handPosition == 0)
        {
            handPosition = 2;
            MoveCursorToPosition(handPosition);
        }
    }

    private void MoveCursorToPosition(int handPosition, bool doNotPlaySound = false)
    {
        switch (handPosition)
        {
            case 0:
                selectorFrame.SetActive(false);
                doneButtonSelectorFrame.SetActive(true);
                handSelector.transform.position = doneButtonHandPoint.position;                
                break;
            case 1:
                selectorFrame.SetActive(true);
                doneButtonSelectorFrame.SetActive(false);
                handSelector.transform.position = healthSkillHandPoint.position;
                selectorFrame.transform.position = healthSkillHandPoint.parent.position;
                break;
            case 2:
                selectorFrame.SetActive(true);
                doneButtonSelectorFrame.SetActive(false);
                handSelector.transform.position = attackSkillHandPoint.position;
                selectorFrame.transform.position = attackSkillHandPoint.parent.position;
                break;
            case 3:
                selectorFrame.SetActive(true);
                doneButtonSelectorFrame.SetActive(false);
                handSelector.transform.position = jumpSkillHandPoint.position;
                selectorFrame.transform.position = jumpSkillHandPoint.parent.position;
                break;
        }

        if (handPosition != 0)
        {
            Image[] images = selectorFrame.GetComponentsInChildren<Image>();
            Animator[] animators = selectorFrame.GetComponentsInChildren<Animator>();            

            if (!CanCurrentSkillBeUpgraded())
            {
                foreach(Animator animator in animators)
                {
                    animator.enabled = false;
                }

                foreach(Image image in images)
                {
                    image.sprite = redSelectorImage;
                }
            }
            else
            {
                foreach (Animator animator in animators)
                {
                    animator.enabled = true;
                }
            }
        }

        if (!doNotPlaySound)
        {
            SoundManager.instance.PlaySound(menuChangeSfx);
        }
    }

    #endregion

    #region Get Directional Values

    private float GetHorizontalValue()
    {
        if (Input.GetButton("Horizontal") || Input.GetButtonDown("Horizontal"))
        {
            return Input.GetAxis("Horizontal");
        }
        else
        {
            float dPadHValue = Input.GetAxis("Horizontal");
            if (dPadHValue != 0f)
            {
                return dPadHValue;
            }
        }

        return 0f;
    }

    private float GetVerticalValue()
    {
        if (Input.GetButton("Vertical") || Input.GetButtonDown("Vertical"))
        {
            return Input.GetAxis("Vertical");
        }
        else
        {
            float dPadVValue = Input.GetAxis("Vertical");
            if (dPadVValue != 0f)
            {
                return dPadVValue;
            }
        }

        return 0f;
    }

    #endregion


}
