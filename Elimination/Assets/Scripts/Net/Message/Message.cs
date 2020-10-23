using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
[Serializable]
public struct BattleUserInfo
{
    public int Uid;
    public int BattleID;
    public int RoleID;
    public BattleUserInfo(int _uid, int _battleID, int _roleID)
    {
        Uid = _uid;
        BattleID = _battleID;
        RoleID = _roleID;
    }
}
public class CommonMessage
{
    public string className;

    public CommonMessage()
    {
        className = GetType().ToString();
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}

public class TcpHeartBeatMessage : CommonMessage
{ 

}

public class TcpLoginMessage : CommonMessage
{
    public string Name;
    public string Pwd;
    public string Token;
    
    public TcpLoginMessage(string name, string pwd,string token) : base()
    {
        Name = name;
        Pwd = pwd;
        Token = token;
    }
}

public class TcpLoginFeedbackMessage : CommonMessage
{
    public enum LoginStatus
    {
        Failed,
        Success,
    }
    public int Uid;
    public int UdpPort;
    public LoginStatus Status;
    public TcpLoginFeedbackMessage(int _uid, int _udpPort, LoginStatus _status) : base()
    {
        Uid = _uid;
        UdpPort = _udpPort;
        Status = _status;
    }
}

public class TcpMatchRequestMessage : CommonMessage
{
    public int UID;
    public int roleID;
    public TcpMatchRequestMessage(int uid, int roleID) : base()
    {
        UID = uid;
        this.roleID = roleID;
    }
}

[Serializable]
public class TcpEnterBattleMessage : CommonMessage
{
    public int Seed;
    public List<BattleUserInfo> BattleUserInfos;
    public TcpEnterBattleMessage(int seed) : base()
    {
        Seed = seed;
        BattleUserInfos = new List<BattleUserInfo>();
    }
    public void Add(BattleUserInfo info)
    {
        BattleUserInfos.Add(info);
    }
}

public class UdpBattleReadyMessage : CommonMessage
{
    public int UID;
    public int BattleID;
    public UdpBattleReadyMessage(int _uid, int _battleID) : base()
    {
        UID = _uid;
        BattleID = _battleID;
    }
}

public class UdpBattleStartMessage : CommonMessage
{
    public UdpBattleStartMessage() : base()
    { 
        
    }
}

public class PlayerOperation : CommonMessage
{
    public int BattleID;
    public int Move;
    public int OperationID;
    public int V1;
    public int V2;
    public PlayerOperation(int _bid, int _move,int _oid) : base()
    {
        BattleID = _bid;
        Move = _move;
        OperationID = _oid;
    }
}