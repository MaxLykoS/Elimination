using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro.EditorUtilities;
using UnityEngine;

public class BattleRoom
{
    private int battleID;
    private List<MatchUserInfo> group;
    private Dictionary<int, int> dic_battleUserUid;
    private Dictionary<int, ServerClientUdp> dic_udp;
    private Dictionary<int, bool> dic_battleReady;

    private bool isBeginBattle = false;
    private int frameNum;
    private int lastFrame;
    private bool _isRun;
    private bool oneGameOver;
    private bool allGameOver;

    private PlayerOperation[] frameOperation;//记录当前帧的玩家操作
    private int[] playerMsgNum;  //记录玩家的包ID
    private bool[] playerGameOver;  //记录玩家游戏结束

    public BattleRoom(int _battleID,List<MatchUserInfo> group)
    {
        this.group = group;
        battleID = _battleID;
    }

    public void CreateBattle()
    {
        int randSeed = UnityEngine.Random.Range(0, 100);

        ThreadPool.QueueUserWorkItem((obj) =>
        {
            dic_battleUserUid = new Dictionary<int, int>();
            dic_udp = new Dictionary<int, ServerClientUdp>();
            dic_battleReady = new Dictionary<int, bool>();

            int userBattleID = 1;

            TcpEnterBattleMessage bm = new TcpEnterBattleMessage(randSeed);
            foreach (MatchUserInfo info in group)
            {
                dic_battleUserUid[info.uid] = userBattleID;

                ServerClientUdp clientUdp = new ServerClientUdp(info.conn.GetIP(),info.uid, HandleMsg);
                clientUdp.StartClientUdp();
                dic_udp[userBattleID] = clientUdp;
                dic_battleReady[userBattleID] = false;

                BattleUserInfo _bUser = new BattleUserInfo(info.uid,userBattleID,info.roleID);
                bm.Add(_bUser);
                userBattleID++;
            }

            foreach (MatchUserInfo info in group)
            {
                Protocol p = new Protocol(bm);
                info.conn.Send(new Protocol(bm));
            }
        });
    }

    private void HandleMsg(ServerClientUdp udp, Protocol proto)
    {
        switch (proto.ClassName)
        {
            case "UdpBattleReadyMessage":
            {
                UdpBattleReadyMessage msg = proto.Decode<UdpBattleReadyMessage>();
                Debug.Log("客户端完成战斗加载");
                //  接收战斗准备
                CheckBattleBegin(msg.BattleID);
                dic_udp[msg.BattleID].RecvClientReady(msg.UID);
            }
            break;
        }
    }

    private void CheckBattleBegin(int _battleID)
    {
        if (isBeginBattle)
            return;
        dic_battleReady[_battleID] = true;

        isBeginBattle = true;
        foreach (var item in dic_battleReady.Values)
        {
            isBeginBattle = (isBeginBattle && item);
        }
        if (isBeginBattle)
        {
            //全部客户端准备完成，开始真正的战斗！
            BeginBattle();
        }
    }

    void BeginBattle()
    {
        frameNum = 0;
        lastFrame = 0;
        _isRun = true;
        oneGameOver = false;
        allGameOver = false;
        Debug.Log("开始第一帧同步");

        int playerNum = dic_battleUserUid.Keys.Count;
        frameOperation = new PlayerOperation[playerNum];
        playerMsgNum = new int[playerNum];
        playerGameOver = new bool[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            frameOperation[i] = null;
            playerMsgNum[i] = 0;
            playerGameOver[i] = false;
        }

        Thread _threadSend = new Thread(Thread_SendFrameData);
        _threadSend.Start();
    }

    private void Thread_SendFrameData()
    {
        //向玩家发送战斗开始
        bool isFinishBS = false;
        while (!isFinishBS)
        {
            UdpBattleStartMessage _bt = new UdpBattleStartMessage();
            Protocol protocol = new Protocol(_bt);
            foreach (var item in dic_udp.Values)
            {
                item.SendMessage(protocol);
            }

            bool _allData = true;
            for (int i = 0; i < frameOperation.Length; i++)
            {
                if (frameOperation[i] == null)
                {
                    _allData = false;  //   有一个玩家没有发送上来操作，则判断为false
                    break;
                }
            }

            if (_allData)
            {
                Debug.Log("战斗服务器：收到全部玩家的第一次操作数据");
                frameNum = 1;

                isFinishBS = true;
            }

            Thread.Sleep(500);
        }

        Debug.Log("开始发送帧数据");

        while (_isRun)
        {

            Thread.Sleep(ServerConfig.FRAME_TIME);
        }

        Debug.Log("帧数据发送线程结束");
    }
}
