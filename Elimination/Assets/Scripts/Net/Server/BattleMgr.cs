using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class BattleMgr
{
    private int battleID = 0;
    private Dictionary<int,BattleRoom> battles = new Dictionary<int, BattleRoom>();
    public BattleMgr()
    {

    }

    public void CreateBattle(List<MatchUserInfo> group)
    { 
        BattleRoom room = new BattleRoom(battleID,group);
        battles[battleID] = room;
        room.CreateBattle();
        battleID++;
        Debug.Log("准备战斗:");
    }

    public void Close()
    {
        foreach (var item in battles.Values)
        {
            item.Close();
        }
    }
}
