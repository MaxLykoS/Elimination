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
    private bool isRun;
    private bool oneGameOver;
    private bool allGameOver;

    private PlayerOperation[] frameOperation;//记录当前帧的玩家操作
    private int[] playerMsgNum;  //记录玩家的包ID
    private bool[] playerGameOver;  //记录玩家游戏结束
    private Dictionary<int, AllPlayerOperation> dic_gameOperation = new Dictionary<int, AllPlayerOperation>();

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
                //  发送BattleEnterMessage
                Protocol p = new Protocol(bm);
                info.conn.Send(new Protocol(bm));
            }
        });
    }

    private void HandleMsg(ServerClientUdp udp, Protocol proto)
    {
        switch (proto.ClassName)
        {
            case nameof(UdpBattleReadyMessage):
            {
                UdpBattleReadyMessage msg = proto.Decode<UdpBattleReadyMessage>();
                Debug.Log("客户端完成战斗加载");
                //  接收战斗准备
                CheckBattleBegin(msg.BattleID);
                dic_udp[msg.BattleID].RecvClientReady(msg.UID);
            }
            break;
            case nameof(UdpUpPlayerOperation):
            {
                UdpUpPlayerOperation msg = proto.Decode<UdpUpPlayerOperation>();
                //Debug.Log("服务器接收到玩家的操作信息");
                UpdatePlayerOperation(msg.PlayerOperation, msg.MsgID);
            }
            break;
            case nameof(UdpUpDeltaFrames):
                {
                    UdpUpDeltaFrames msg = proto.Decode<UdpUpDeltaFrames>();

                    UdpDownDeltaFrames _downData = new UdpDownDeltaFrames();

                    for (int i = 0; i < msg.Frames.Count; i++)
                    {
                        int frameIndex = msg.Frames[i];
                        UdpDownFrameOperations _downOps = new UdpDownFrameOperations();
                        _downOps.FrameID = frameIndex;
                        _downOps.Ops = dic_gameOperation[frameIndex];

                        _downData.FramesData.Add(_downOps);
                    }

                    dic_udp[msg.BattleID].SendMessage(new Protocol(_downData));
                }
                break;
            default:
                {
                    Debug.LogError("未知客户端UDP信息");
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
        isRun = true;
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

        while (isRun)
        {
            UdpDownFrameOperations _ops = new UdpDownFrameOperations();
            //Debug.Log("服务器转发帧数据");
            if (oneGameOver)
            {
                _ops.FrameID = lastFrame;
                _ops.Ops = dic_gameOperation[lastFrame];
            }
            else
            {
                _ops.Ops.Operations.AddRange(frameOperation);
                _ops.FrameID = frameNum;
                dic_gameOperation[frameNum] = _ops.Ops;
                lastFrame = frameNum;
                frameNum++;

                Protocol protocol = new Protocol(_ops);
                foreach (var item in dic_udp)
                {
                    int _index = item.Key - 1;
                    if (!playerGameOver[_index])
                        item.Value.SendMessage(protocol);
                }
            }

            Thread.Sleep(ServerConfig.FRAME_TIME);
        }

        Debug.Log("帧数据发送线程结束.....................");
    }

    public void UpdatePlayerOperation(PlayerOperation _op, int _msgNum)
    {
        int _index = _op.BattleID;
        if (_msgNum > playerMsgNum[_index]) //  如果消息ID小于客户端，则服务器丢帧，赋值补足
        {
            frameOperation[_index] = _op;
            playerMsgNum[_index] = _msgNum;
        }
        else
        { 
            //  早期的包就不记录了
        }
    }

    public void Close()
    {
        foreach (var item in dic_udp.Values)
        {
            item.CloseUdpClient();
        }
        isRun = false;
    }
}
