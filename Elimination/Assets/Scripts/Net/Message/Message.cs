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
    public string Words;
    public LoginMessage(string name, string words) : base()
    {
        Name = name;
        Words = words;
    }
}

public class MatchMessage : CommonMessage
{
    public string GameID;
    public MatchMessage(string id) : base()
    {
        GameID = id;
    }
}
