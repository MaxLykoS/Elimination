using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData 
{
    public static int RandSeed;  //    随机种子
    public static int BattleID;

    public List<BattleUserInfo> list_battleUser;
    private static BattleData instance;
    public static BattleData Instance
    {
        get
        {
            if (instance == null)
                instance = new BattleData();
            return instance;
        }
    }

    private BattleData()
    { 
        
    }

    public void UpdateBattleInfo(int _randSeed, List<BattleUserInfo> _userInfo)
    {
        //Debug.Log("更新战场信息");
        RandSeed = _randSeed;
        list_battleUser = new List<BattleUserInfo>(_userInfo);

        foreach (BattleUserInfo info in list_battleUser)
        {
            if (info.Uid == ClientGlobal.UID)
            {
                BattleID = info.BattleID;
                Debug.Log("自己的战斗ID:" + BattleID);
            }
        }
    }
}
