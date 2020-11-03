using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class ClientUdp
{
    //  返回给游戏的delegate
    public delegate void DelegateReceiveMessage<T>(T message);
    
    public DelegateReceiveMessage<UdpDownFrameOperations> msg_frame_operation { get; set; }
    public DelegateReceiveMessage<UdpBattleStartMessage> msg_battle_start { get; set; }
    public DelegateReceiveMessage<UdpDownDeltaFrames> msg_delta_frame { get; set; }

    private UdpClient sendClient = null;
    private int localPort;
    private bool isRun;
    private bool isRecv;

    private int userUid;

    public void StartClientUdp()
    {
        if (isRun)
        {
            Debug.LogError("客户端udp已经启动");
            return;
        }
        isRun = true;

        CreateUpd();
        StartRecvMessage();
    }

    void CreateUpd()
    {
        sendClient = new UdpClient();
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ClientGlobal.ServerIP), ClientGlobal.UdpSendPort);
        sendClient.Connect(endpoint);
        IPEndPoint _localEnd = (IPEndPoint)sendClient.Client.LocalEndPoint;
        localPort = _localEnd.Port;  //  初始化localPort
        Debug.Log("UDP参数" + _localEnd.Address + "," + _localEnd.Port);
    }

    private void StartRecvMessage()
    {
        Thread t = new Thread(new ThreadStart(RecvThread));
        t.Start();
    }

    private void RecvThread()
    {
        isRecv = true;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ClientGlobal.ServerIP), localPort);
        while (isRecv)
        {
            try
            {
                if (sendClient.Available > 0)
                {
                    byte[] buf = sendClient.Receive(ref endpoint);
                    Protocol protocol = new Protocol(ref buf);
                    HandleMsg(protocol);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("udpClient接收数据异常:" + ex.Message);
            }
        }
        Debug.Log("udp接收线程退出");
    }

    private void HandleMsg(Protocol protocol)
    {
        switch (protocol.ClassName)
        {
            case nameof(UdpBattleStartMessage):
                {
                    Debug.Log("服务器知道了客户端完成了加载");
                    ClientGlobal.Instance.AddAction(() =>
                    {
                        msg_battle_start(protocol.Decode<UdpBattleStartMessage>());
                    });
                }
                break;
            case nameof(UdpDownFrameOperations):
                {
                    //Debug.Log("客户端收到服务器转发的帧操作信息");
                    ClientGlobal.Instance.AddAction(() =>
                    {
                        msg_frame_operation(protocol.Decode<UdpDownFrameOperations>());
                    });
                }
                break;
            case nameof(UdpDownDeltaFrames):
                {
                    Debug.Log("客户端根据服务器转发的旧帧信息处理");
                    UdpDownDeltaFrames msg = protocol.Decode<UdpDownDeltaFrames>();
                    ClientGlobal.Instance.AddAction(() =>
                    {
                        msg_delta_frame(msg);
                    });
                }
                break;
            default:Debug.Log("未查找到udp信息处理函数:" + protocol.ClassName);
                break;
        }
    }

    public void StopRecvMessage()
    {
        isRecv = false;
    }

    public void EndClientUdp()
    {
        try
        {
            isRun = false;
            isRecv = false;
            sendClient.Close();
            sendClient = null;
        }
        catch (Exception ex)
        {
            Debug.LogError("udp连接关闭异常" + ex.Message);
        }
    }

    public void Destroy()
    {
        msg_delta_frame = null;
        msg_frame_operation = null;
        EndClientUdp();
    }

    public void SendMessage(Protocol protocol)
    {
        if (isRun)
        {
            try
            {
                byte[] buff = protocol.Encode();
                sendClient.Send(buff, buff.Length);
            }
            catch (Exception ex)
            {
                Debug.Log("UDP发送失败:" + ex.Message);
            }
        }
    }
}
