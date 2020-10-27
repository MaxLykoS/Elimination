using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMgr : MonoBehaviour
{
    public bool initFinish;

    

    public void InitData()
    { 
        
    }

    public void Logic_Operation(AllPlayerOperation _op)
    {
        Debug.Log("更新操作：按键为" + _op.Operations[0].Keyinput);
    }
}
