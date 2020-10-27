using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BattleConn : Singletion<BattleConn>
{
    private bool isBattleStart;
    private bool isBattleFinish;
    private int msgNum = 0; //计数器

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
        clientUdp.msg_battle_start = Message_Battle_Start;
        clientUdp.msg_frame_operation = Message_Frame_Operation;
        clientUdp.msg_delta_frame = Message_Delta_Frame_Data;

        isBattleStart = false;
        StartCoroutine(WaitInitData());
    }

    IEnumerator WaitInitData()
    {
        yield return new WaitUntil(() =>
        {
            return roleMgr.initFinish&&obstacleMgr.initFinish;
        });
        this.InvokeRepeating(nameof(Send_BattleReady), 0.5f, 0.2f);
    }

    void Send_BattleReady()
    {
        Debug.Log("发送BattleReady");
        UdpBattleReadyMessage rm = new UdpBattleReadyMessage(ClientGlobal.UID, BattleData.BattleID);
        clientUdp.SendMessage(new Protocol(rm));
    }

    void Message_Battle_Start(UdpBattleStartMessage msg)
    {
        if (isBattleStart)
            return;

        isBattleStart = true;
        this.CancelInvoke(nameof(Send_BattleReady));

        float _time = ClientGlobal.FrameTime * 0.001f; // 66ms
        this.InvokeRepeating(nameof(Send_Operation), _time, _time);  // 循环调用Send_Operation方法

        StartCoroutine(WaitForFirstMessage());
    }

    void Message_Frame_Operation(UdpDownFrameOperations msg)
    {
        //Debug.Log("客户端处理服务器发来的帧操作信息");
        BattleData.Instance.AddNewFrameData(msg.FrameID, msg.Ops);
    }

    void Send_Operation()
    {
        msgNum++;
        UdpUpPlayerOperation _op = new UdpUpPlayerOperation(msgNum,BattleData.Instance.SelfOperation);
        clientUdp.SendMessage(new Protocol(_op));

        BattleData.Instance.ResetSelfOperation();
    }

    IEnumerator WaitForFirstMessage()
    {
        yield return new WaitUntil(() =>
        {
            return BattleData.Instance.GetFrameDataNum() > 0;   //  在这里等待第一帧，第一帧没更新之前不会做更新
        });
        this.InvokeRepeating(nameof(LogicUpdate), 0f, 0.020f);
    }

    void LogicUpdate()  //  通过接收的数据帧来更新逻辑显示
    {
        AllPlayerOperation _op;
        if (BattleData.Instance.TryGetNextPlayerOp(out _op))
        {
            roleMgr.Logic_Operation(_op);
            //  更新逻辑显示

            BattleData.Instance.RunOpSucces();
        }
    }

    public void SendDeltaFrames(int _battleID, List<int> list)
    {
        Debug.Log("正在发送缺失数据帧");
        UdpUpDeltaFrames _msg = new UdpUpDeltaFrames(_battleID, list);

        clientUdp.SendMessage(new Protocol(_msg));
    }

    void OnDestroy()
    {
        BattleData.Instance.ClearData();
        clientUdp.Destroy();
    }

    void Message_Delta_Frame_Data(UdpDownDeltaFrames msg)
    {
        if (msg.FramesData.Count > 0)
        {
            foreach (var item in msg.FramesData)
            {
                BattleData.Instance.AddLackFrameData(item.FrameID, item.Ops);
            }
        }
    }

    void Update()
    {
        KeyInputMgr.Instance.UpdateInput();
    }
}
