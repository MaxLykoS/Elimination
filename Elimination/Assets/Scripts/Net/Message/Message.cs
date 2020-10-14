using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

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

public class HeartBeatMessage : CommonMessage
{ 

}

public class LoginMessage : CommonMessage
{
    public string Name;
    public string Pwd;
    
    public LoginMessage(string name, string pwd) : base()
    {
        Name = name;
        Pwd = pwd;
    }
}

public class LoginFeedbackMessage : CommonMessage
{
    public enum LoginStatus
    {
        Failed,
        Success,
    }
    public string IP;
    public string Port;
    public LoginStatus Status;
    public LoginFeedbackMessage(string ip, string port, LoginStatus status) : base()
    {
        IP = ip;
        Port = port;
        Status = status;
    }
}

public class MatchMessage : CommonMessage
{
    public int RandsSeed;
    public MatchMessage() : base()
    {
        
    }

    public MatchMessage(int seed) : base()
    {
        RandsSeed = seed;
    }
}
