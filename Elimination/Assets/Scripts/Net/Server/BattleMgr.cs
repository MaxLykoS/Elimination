using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr
{
    private List<BattleRoom> battles = new List<BattleRoom>();
    public BattleMgr()
    { 
        
    }

    public void BeginBattle(List<Conn> group)
    {
        BattleRoom room = new BattleRoom(group);
        battles.Add(room);
        room.Begin();
        Debug.Log("开始战斗");
    }
}
