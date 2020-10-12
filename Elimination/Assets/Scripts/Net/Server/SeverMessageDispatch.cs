using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SeverMessageDispatch
{
    public delegate void HandleMsg(Conn conn,Protocol proto);
    private Dictionary<string, HandleMsg> dispatchDic = new Dictionary<string, HandleMsg>();

    public SeverMessageDispatch()
    {
        dispatchDic.Add(typeof(HeartBeatMessage).ToString(), HeartBeatMessage);
        dispatchDic.Add(typeof(LoginMessage).ToString(), LoginMessage);
        dispatchDic.Add(typeof(MatchMessage).ToString(), MatchMessage);
    }

    public void Dispatch(Conn conn,Protocol proto)
    {
        dispatchDic[proto.GetClassName()](conn,proto);
    }

    #region 监听数据包
    private void HeartBeatMessage(Conn conn, Protocol proto)
    {
        conn.lastTickTime = Utility.GetTimeStamp();
        Debug.Log("[更新心跳时间]" + conn.GetAdress());
    }

    private void LoginMessage(Conn conn, Protocol proto)
    {
        LoginMessage login = proto.Decode<LoginMessage>();
        Debug.Log("登陆玩家:" + login.Name + " 登录信息:" + login.Pwd);
        login.Status = global::LoginMessage.LoginStatus.Success;
        conn.Send(new Protocol(login));
    }

    private void MatchMessage(Conn conn, Protocol proto)
    { 
        
    }
    #endregion
}
