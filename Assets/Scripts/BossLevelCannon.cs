using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelCannon : Cannon
{
    private new void Update()
    {
        if (BossLevelManager.instance.IsCannonTurn)
        {
            base.Update();
        }
    }

    public void DestroyControllingPig()
    {
        Destroy(controllingPig);
    }
}
