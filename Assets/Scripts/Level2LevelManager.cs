using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2LevelManager : MonoBehaviour
{
    public AudioClip level2Music;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.StopMusic();
        SoundManager.instance.FadeInMusic(level2Music, 0.3f);
    }


}
