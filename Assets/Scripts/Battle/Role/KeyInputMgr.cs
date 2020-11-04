using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputMgr : Singletion<KeyInputMgr>
{
    public void UpdateInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            BattleData.Instance.UpdateKeyInput('W');
        }
        else if (Input.GetKey(KeyCode.S))
        {
            BattleData.Instance.UpdateKeyInput('S');
        }
        else if (Input.GetKey(KeyCode.A))
        {
            BattleData.Instance.UpdateKeyInput('A');
        }
        else if (Input.GetKey(KeyCode.D))
        {
            BattleData.Instance.UpdateKeyInput('D');
        }
    }
}
