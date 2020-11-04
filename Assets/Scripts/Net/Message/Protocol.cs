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

    public Protocol(ref byte[] readbuff,int start,int length)  // for tcp
    {
        json = Encoding.UTF8.GetString(readbuff, start, length);
        common = JsonUtility.FromJson<CommonMessage>(json);
        className = common.className;
    }

    public Protocol(ref byte[] readbuf)  //  for udp
    {
        json = Encoding.UTF8.GetString(readbuf);
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

    public Protocol(string json)
    {
        this.json = json;
        className = JsonUtility.FromJson<CommonMessage>(json).className;
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

    public string Json
    {
        get { return json; }
    }

    public string ClassName
    {
        get { return className; }
    }
}
