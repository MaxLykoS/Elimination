using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MatchMgr
{
    const int MATCH_COUNT = 1;
    private List<Conn> match_Queue = new List<Conn>();

    private static readonly object mmlockObj = new object();
    private static MatchMgr instance = null;

    private List<Action> list_action = new List<Action>();
    private Mutex mutex_actionList = new Mutex();

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

    public void AddMatchingPlayer(Conn conn)
    {
        match_Queue.Add(conn);

        if (match_Queue.Count >= MATCH_COUNT)
        {
            List<Conn> connGroup = new List<Conn>();
            for (int i = 0; i < MATCH_COUNT; i++)
            {
                connGroup.Add(match_Queue[0]);
                match_Queue.RemoveAt(0);
            }

            AddAction(()=>
            {
                BeginBattle(connGroup);
            });
        }
    }

    private void BeginBattle(List<Conn> connGroup)
    { 
        
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
