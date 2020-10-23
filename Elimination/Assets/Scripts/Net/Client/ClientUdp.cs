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
    public delegate void DelegateReceiveMessage<T>(T message);

    public DelegateReceiveMessage<UdpBattleStartMessage> mes_battle_start { get; set; }

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
                byte[] buf = sendClient.Receive(ref endpoint);
                Protocol protocol = new Protocol(ref buf);
                HandleMsg(protocol);
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
            case "UdpBattleStartMessage":
                Debug.Log("服务器知道了客户端完成了加载");
                ClientGlobal.Instance.AddAction(() =>
                {
                    mes_battle_start(protocol.Decode<UdpBattleStartMessage>());
                });
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

    void OnDestroy()
    {
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
