using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ServerMessageDispatch
{
    public delegate void HandleMsg(Conn conn,Protocol proto);
    private Dictionary<string, HandleMsg> dispatchDic = new Dictionary<string, HandleMsg>();

    public ServerMessageDispatch()
    {
        dispatchDic.Add(typeof(TcpHeartBeatMessage).ToString(), HeartBeatMessage);
        dispatchDic.Add(typeof(TcpLoginMessage).ToString(), LoginMessage);
        dispatchDic.Add(typeof(TcpMatchRequestMessage).ToString(), MatchRequestMessage);
    }

    public void Dispatch(Conn conn,Protocol proto)
    {
        dispatchDic[proto.ClassName](conn,proto);
    }

    #region 监听数据包
    private void HeartBeatMessage(Conn conn, Protocol proto)
    {
        conn.lastTickTime = Utility.GetTimeStamp();
        Debug.Log("[更新心跳时间]" + conn.GetAdress());
    }

    private void LoginMessage(Conn conn, Protocol proto)
    {
        TcpLoginMessage login = proto.Decode<TcpLoginMessage>();
        Debug.Log("登陆玩家:" + login.Name + " 登录信息:" + login.Pwd + "客户端凭证:" + login.Token);

        int _uid = UserMgr.Instance.UserLogin(login.Token, conn.GetAdress());


        TcpLoginFeedbackMessage fb = new TcpLoginFeedbackMessage(_uid, ServerConfig.UDP_RECV_PORT, 
                                        TcpLoginFeedbackMessage.LoginStatus.Success);
        conn.Send(new Protocol(fb));
    }

    private void MatchRequestMessage(Conn conn, Protocol proto)
    {
        TcpMatchRequestMessage rm = proto.Decode<TcpMatchRequestMessage>();
        Debug.Log("服务器收到请求匹配信息");
        MatchMgr.Instance.AddMatchingPlayer(conn, rm.UID,rm.UID);
    }
    #endregion
}
