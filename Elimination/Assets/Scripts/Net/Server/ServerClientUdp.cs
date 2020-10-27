using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerClientUdp
{
    public delegate void HandleMsg(ServerClientUdp udp, Protocol proto);
    private HandleMsg handleMsg;

    public int userUid;
    private string clientIP;
    private int sendPort;
    private int recvPort;
    private UdpClient sendClient = null;
    private IPEndPoint sendEndPoint;
    private bool isRunning;

    public ServerClientUdp(string _ip,int _uid,HandleMsg handleMsgDelegate)
    {
        clientIP = _ip;
        userUid = _uid;
        this.handleMsg = handleMsgDelegate;
    }

    public void StartClientUdp()
    {
        if (sendEndPoint != null)
        {
            Debug.Log("客户端udp已经启动");
            return;
        }

        isRunning = true;

        sendClient = new UdpClient(ServerConfig.UDP_RECV_PORT,AddressFamily.InterNetwork);
        IPEndPoint _localip = (IPEndPoint)sendClient.Client.LocalEndPoint;
        Debug.Log("UDP端口" + _localip.Port);
        recvPort = _localip.Port;

        Thread t = new Thread(new ThreadStart(RecvThread));
        t.Start();
    }

    public void CloseUdpClient()
    {
        try
        {
            isRunning = false;
            sendClient.Close();
            sendEndPoint = null;
            sendClient = null;
        }
        catch (Exception ex)
        {
            Debug.Log("UDP连接关闭异常:" + ex.Message);
        }
    }

    private void CreateSendEndPort(int _port)
    {
        Debug.Log("udp客户端ip:" + clientIP);
        sendEndPoint = new IPEndPoint(IPAddress.Parse(clientIP), _port);
    }

    public void SendMessage(Protocol proto)
    {
        if (isRunning)
        {
            try
            {
                byte[] bytes = proto.Encode();
                sendClient.Send(bytes, bytes.Length, sendEndPoint);
            }
            catch (Exception ex)
            {
                Debug.Log("UDP发送失败:" + ex.Message);
            }
        }
    }

    private void RecvThread()
    {
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(clientIP), recvPort);
        while (isRunning)
        {
            //try
            {
                //Debug.Log("接收客户端udp消息");
                if (sendClient == null)
                    Debug.Log("sendClient空了");
                byte[] buf = sendClient.Receive(ref endpoint);
                if (sendEndPoint == null)
                {
                    sendPort = endpoint.Port;
                }
                string message = Encoding.UTF8.GetString(buf);
                handleMsg(this, new Protocol(message));
            }
            /*catch (Exception ex)
            {
                Debug.Log("接收udp信息异常" + ex.Message);
            }*/
        }
        Debug.Log("UDP接收线程退出");
    }

    public void RecvClientReady(int _userUid)
    {
        if (_userUid == userUid && sendEndPoint == null)
        {
            CreateSendEndPort(sendPort);
        }
    }
}
