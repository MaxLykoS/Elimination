using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Protocol
{
    //传输的字符串
    private string className;
    private CommonMessage common;
    private string json;

    //解码器,将字节流转换为字符串
    public Protocol(ref byte[] readbuff,int start,int length)
    {
        json = Encoding.UTF8.GetString(readbuff, start, length);
        common = JsonUtility.FromJson<CommonMessage>(json);
        className = common.className;
    }

    public Protocol(CommonMessage message)
    {
        common = message;
        className = message.className;
        json = JsonUtility.ToJson(message);
    }

    public Protocol()
    { 
        
    }

    public CommonMessage GetCommonMsg()
    {
        return common;
    }

    public T Decode<T>() where T : CommonMessage
    {
        return JsonUtility.FromJson<T>(json);
    }

    public byte[] Encode()
    { 
        return Encoding.UTF8.GetBytes(json);
    }

    public string GetJson()
    {
        return json;
    }

    public string GetClassName()
    {
        return className;
    }
}
