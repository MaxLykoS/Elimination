using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConn : Singletion<BattleConn>
{
    private bool isBattleStart;
    private bool isBattleFinish;

    [HideInInspector]
    public RoleMgr roleMgr;
    [HideInInspector]
    public ObstacleMgr obstacleMgr;
    private ClientUdp clientUdp;

    void Start()
    {
        GameObject go = GameObject.Find("ClientLogic");
        roleMgr = go.GetComponent<RoleMgr>();
        obstacleMgr = go.GetComponent<ObstacleMgr>();

        clientUdp = new ClientUdp();
        clientUdp.StartClientUdp();
        clientUdp.mes_battle_start = Message_Battle_Start;

        isBattleStart = false;
        StartCoroutine(WaitInitData());
    }

    IEnumerator WaitInitData()
    {
        yield return new WaitUntil(() =>
        {
            return roleMgr.initFinish&&obstacleMgr.initFinish;
        });
        this.InvokeRepeating("Send_BattleReady", 0.5f, 0.2f);
    }

    void Send_BattleReady()
    {
        UdpBattleReadyMessage rm = new UdpBattleReadyMessage(ClientGlobal.UID, BattleData.BattleID);
        clientUdp.SendMessage(new Protocol(rm));
    }

    void Message_Battle_Start(UdpBattleStartMessage msg)
    {
        if (isBattleStart)
            return;

        isBattleStart = true;
        this.CancelInvoke("Send_battleReady");

        float _time = ClientGlobal.FrameTime * 0.001f; // 66ms
        this.InvokeRepeating("Send_Operation", _time, _time);  // 循环调用Send_Operation方法

        //StartCoroutine(WaitForFirstMessage);
        Debug.Log("代码写到这里了");
    }

    void Send_Operation()
    { 
        
    }

    /*IEnumerator WaitForFirstMessage()
    { 
        
    }*/
}
