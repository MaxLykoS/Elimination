using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class ClientTcp
{ 
    const int BUFFER_SIZE = 1024;

    private static ClientTcp instance;
    public static ClientTcp Instance { get { return instance; } }

    //  心跳时间
    public float LastTickTime = 0;
    const float HEART_BEAT_TIME = 3;

    private Socket socket;
    private byte[] readBuffer = new byte[BUFFER_SIZE];
    private int bufferCount = 0;

    //  粘包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(Int32)];

    //  协议
    public Protocol proto;

    //  消息分发
    private ClientMessageDispatch dispatch = new ClientMessageDispatch();

    //  状态
    public enum Status
    { 
        None,
        Connected,
    }
    public Status status = Status.None;

    public ClientTcp()
    {
        instance = this;
    }

    public bool Connect(string host, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
            socket.BeginReceive(readBuffer, bufferCount, BUFFER_SIZE - bufferCount, SocketFlags.None, ReceiveCB, readBuffer);
            Debug.Log("连接成功");

            status = Status.Connected;
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("连接服务器失败" + ex.Message);
            return false;
        }
    }

    private void ReceiveCB(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            bufferCount += count;
            ProcessData();
            socket.BeginReceive(readBuffer, bufferCount, BUFFER_SIZE - bufferCount, SocketFlags.None, ReceiveCB, readBuffer);
        }
        catch (Exception ex)
        {
            Debug.Log("异步接受回调失败" + ex.Message);
            status = Status.None;
        }
    }

    private void ProcessData()
    {
        //粘包处理
        if (bufferCount < sizeof(Int32))
            return;

        //包体长度
        Array.Copy(readBuffer, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (bufferCount < msgLength + sizeof(Int32))
            return;

        //协议解码
        Protocol protocol = new Protocol(ref readBuffer, sizeof(Int32), msgLength);
        //Debug.Log("收到消息" + protocol.GetDesc());

        Queue<Protocol> msgQueue = dispatch.MsgQueue;
        lock (msgQueue)
            msgQueue.Enqueue(protocol);

        //清除已处理的消息
        int count = bufferCount - msgLength - sizeof(Int32);
        Array.Copy(readBuffer, msgLength + sizeof(Int32), readBuffer, 0, count);
        bufferCount = count;
        if (bufferCount > 0)
            ProcessData();
    }

    public bool Send(Protocol protocol)
    {
        if (status != Status.Connected)
        {
            Debug.LogError("还没建立连接无法发送数据");
            return false;
        }

        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        socket.Send(sendbuff);
        Debug.Log("发送消息");
        return true;
    }

    public bool Send(Protocol protocol, ClientMessageDispatch.ListenDelegate cb)
    {
        string className = protocol.GetClassName();
        return Send(protocol, className, cb);
    }

    private bool Send(Protocol protocol, string className, ClientMessageDispatch.ListenDelegate cb)
    {
        if (status != Status.Connected)
            return false;
        dispatch.AddOnceListener(className, cb);
        return Send(protocol);
    }

    public bool Close()
    {
        try
        {
            socket.Close();
            status = Status.None;
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("断开连接失败" + ex.Message);
            return false;
        }
    }

    //  心跳检测
    public void Update()
    {
        dispatch.Update();
        if (status == Status.Connected)
        {
            if (Time.time - LastTickTime > HEART_BEAT_TIME)
            {
                Protocol protocol = new Protocol(new HeartBeatMessage());
                Send(protocol);
                LastTickTime = Time.time;
            }
        }
    }
}
