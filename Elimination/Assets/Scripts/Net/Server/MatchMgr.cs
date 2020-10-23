using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public struct MatchUserInfo
{
    public int uid;
    public int roleID;
    public Conn conn;

    public MatchUserInfo(int _uid, int _roleID, Conn conn)
    {
        uid = _uid;
        roleID = _roleID;
        this.conn = conn;
    }
}
public class MatchMgr
{
    private List<MatchUserInfo> match_Queue = new List<MatchUserInfo>();

    private static readonly object mmlockObj = new object();
    private static MatchMgr instance = null;

    private List<Action> list_action = new List<Action>();
    private Mutex mutex_actionList = new Mutex();

    private BattleMgr battleMgr = new BattleMgr();

    public static MatchMgr Instance
    {
        get
        {
            lock (mmlockObj)
            {
                if (instance == null)
                {
                    instance = new MatchMgr();
                }
            }
            return instance;
        }
    }

    private MatchMgr()
    { 
        
    }

    public void AddMatchingPlayer(Conn conn,int uid,int roleID)
    {
        match_Queue.Add(new MatchUserInfo(uid,roleID,conn));

        if (match_Queue.Count >= ServerConfig.BattleUserNum)
        {
            List<MatchUserInfo> group = new List<MatchUserInfo>();
            for (int i = 0; i < ServerConfig.BattleUserNum; i++)
            {
                group.Add(match_Queue[0]);
                match_Queue.RemoveAt(0);
            }

            AddAction(()=>
            {
                battleMgr.CreateBattle(group);
            });
        }
    }

    private void AddAction(Action _action)
    {
        mutex_actionList.WaitOne();
        list_action.Add(_action);
        mutex_actionList.ReleaseMutex();
    }

    public void DoForAction()
    {
        mutex_actionList.WaitOne();
        for (int i = 0; i < list_action.Count; i++)
            list_action[i]();
        list_action.Clear();
        mutex_actionList.ReleaseMutex();
    }
}
