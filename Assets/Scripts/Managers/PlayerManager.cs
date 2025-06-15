using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public int diamondsAmount = 0;
    public int playerHealth = 3;
    /// <summary>
    /// The SkillsList will be bound to the character manager so that
    /// it is valid and accessible during the whole game session.
    /// 
    /// The values utilized during the SkillList creation can be found in the Unity Editor
    /// in the prefab PlayerManager (Assets\Prefabs\PlayerManager)
    /// </summary>
    public SkillsList SkillsList { get; private set; }
    public string NextLevel { get; set; }
    public bool IsInMenu { get; set; } = false;
    public bool HasAlreadySeenBossCamera { get; set; } = false;
    public bool IsEasyMode { get; set; } = false;
    public bool IsEnteringLevel1ForTheFirstTime { get; set; } = true;

    private static bool isCreated = false;

    // Start is called before the first frame update
    void Awake()
    {        
        if (isCreated)
        {
            Destroy(gameObject);
        }
        
        isCreated = true;
        DontDestroyOnLoad(gameObject);

        SkillsList = GetComponent<SkillsList>();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevel, LoadSceneMode.Single);
        if (NextLevel.Equals("Menu"))
        {
            ResetPlayerManager();
        }
    }

    public void ResetPlayerManager()
    {
        diamondsAmount = 0;
        playerHealth = 3;
        NextLevel = "";
        SkillsList.ResetAllSkills();
        IsEnteringLevel1ForTheFirstTime = true;
        HasAlreadySeenBossCamera = false;
    }   

    public void PrepareEasyMode()
    {
        ResetPlayerManager();
        IsEasyMode = true;
    }

    public void PrepareNormalMode()
    {
        ResetPlayerManager();
        IsEasyMode = false;
    }
}
