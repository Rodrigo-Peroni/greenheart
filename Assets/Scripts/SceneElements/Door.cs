using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public bool isOpened = false;
    public bool isStartingDoor;
    public string nextLevel;
    public bool opensMenu = true;

    private Animator animator;
    private Player player;    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();

        if (isOpened)
        {
            animator.SetTrigger("InstantOpenDoor");
            Invoke("ActivateDoor", 1.2f);
        }
    }

    public void ActivateDoor()
    {        
        if (isOpened)
        {
            animator.SetTrigger("Close");
            isOpened = false;
        }
        else
        {
            animator.SetTrigger("Open");
            isOpened = true;

            Invoke("LoadSkillScreen", 1.2f);
        }
    }

    private void LoadSkillScreen()
    {
        player.playerManager.diamondsAmount = player.DiamondsCount;
        player.playerManager.playerHealth = player.health;
        player.playerManager.NextLevel = nextLevel;
        Hud.instance.TurnOnBlur();
        if (!isStartingDoor)
        {
            if (opensMenu)
            {
                Hud.instance.ShowSkillsMenu();
            }
            else
            {
                player.playerManager.LoadNextLevel();
            }
        }
        else
        {
            Hud.instance.ShowLevelChangeMenu();
        }        
    }
}
