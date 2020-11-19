using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Conn
{
    public static readonly int BUFFER_SIZE = 1024;
    public Socket socket;
    public bool isUse = false;
    
    //  Buffer
    public byte[] readBuff;
    public int bufferCount = 0;

    //  粘包分包
    public byte[] lenBytes;
    public Int32 msgLength = 0;

    //  心跳时间
    public long lastTickTime = long.MinValue;

    //  在连接池中的下标
    public int index;

    public Conn(int index)
    {
        readBuff = new byte[BUFFER_SIZE];
        lenBytes = new byte[sizeof(UInt32)];
        this.index = index;
    }

    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        bufferCount = 0;

        //  心跳处理
        lastTickTime = Utility.GetTimeStamp();
    }

    public string GetAdress()
    {
        if (!isUse)
            return "无法获取地址";
        return socket.RemoteEndPoint.ToString();
    }

    public string GetIP()
    {
        if (!isUse)
            return "无法获取地址";
        return socket.RemoteEndPoint.ToString().Split(':')[0];
    }

    //  关闭连接
    public void Close()
    {
        if (!isUse)
            return;
        Debug.Log("断开连接" + GetAdress());
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        isUse = false;
    }

    //  发送消息
    public void Send(Protocol proto)
    {
        byte[] bytes = proto.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendBuff = length.Concat(bytes).ToArray();

        try
        {
            socket.BeginSend(sendBuff, 0, sendBuff.Length, SocketFlags.None, null, null);
        }
        catch (Exception ex)
        {
            Debug.Log("[发送消息]" + GetAdress() + ex.Message);
        }
    }

    public int BufferRemain()
    {
        return BUFFER_SIZE - bufferCount;
    }

    //  检查获得的信息长度是否大于（长度+等长的数据）
    public bool ProcessMsgLen()
    {
        //  小于长度字节
        if (bufferCount < sizeof(Int32))
            return false;

        //  获得消息长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);

        if (bufferCount < msgLength + sizeof(Int32))
            return false;

        return true;
    }

    public Protocol Translate()
    {
        return new Protocol(ref readBuff,sizeof(Int32), msgLength);
    }

    public void ClearLastMsg()
    {
        //  3 A B C 4 A B
        //  0 1 2 3 4 5 6
        int nextLen = bufferCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, nextLen);
        bufferCount = nextLen;    
    }
}
