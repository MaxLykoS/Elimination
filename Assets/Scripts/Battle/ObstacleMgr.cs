using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMgr : MonoBehaviour
{
    public bool initFinish;

    private void Start()
    {
        initFinish = true;
    }

    public void Logic_ObsUpdate()
    {
        Obj.UpdateBound();
    }
}
