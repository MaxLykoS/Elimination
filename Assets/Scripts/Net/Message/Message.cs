using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
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
    public TcpMatchRequestMessage(int uid) : base()
    {
        UID = uid;
    }
}

[Serializable]
public struct BattleUserInfo
{
    public int Uid;
    public int BattleID;
    public BattleUserInfo(int _uid, int _battleID)
    {
        Uid = _uid;
        BattleID = _battleID;
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

[Serializable]
public class PlayerOperation
{
    public int BattleID;
    public int Move;
    public int OperationID;
    public string Keyinput;
    public PlayerOperation()
    { 
        
    }
}

[Serializable]
public class AllPlayerOperation
{
    public List<PlayerOperation> Operations;
    public AllPlayerOperation()
    {
        Operations = new List<PlayerOperation>();
    }
}

[Serializable]
public class UdpUpPlayerOperation : CommonMessage
{
    public int MsgID;
    public PlayerOperation PlayerOperation;
    public UdpUpPlayerOperation(int _mid, PlayerOperation _op) : base()
    {
        MsgID = _mid;
        PlayerOperation = _op;
    }
}

[Serializable]
public class UdpDownFrameOperations : CommonMessage
{
    public int FrameID;
    public AllPlayerOperation Ops;
    public UdpDownFrameOperations() : base()
    {
        Ops = new AllPlayerOperation();
    }
}

[Serializable]
public class UdpUpDeltaFrames : CommonMessage
{
    public int BattleID;
    public List<int> Frames;
    public UdpUpDeltaFrames(int _bid, List<int> _frames) : base()
    {
        BattleID = _bid;
        Frames = new List<int>();
        Frames.AddRange(_frames);
    }
}

public class UdpDownDeltaFrames : CommonMessage
{
    public List<UdpDownFrameOperations> FramesData;
    public UdpDownDeltaFrames() : base()
    {
        FramesData = new List<UdpDownFrameOperations>();
    }
}