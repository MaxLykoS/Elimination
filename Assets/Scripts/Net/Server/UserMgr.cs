using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserInfo
{
    public string socketIp;
    public bool isLogin;

    public UserInfo(string _socketIp, bool _isLogin)
    {
        this.socketIp = _socketIp;
        this.isLogin = _isLogin;
    }
}
public class UserMgr
{
    private static readonly object umlockObj = new object();
    private static UserMgr instance = null;

    private int userUid;
    private Dictionary<int, UserInfo> dic_userInfo;
    private Dictionary<string, int> dic_tokenUid;

    public static UserMgr Instance
    {
        get
        {
            lock (umlockObj)
            {
                if (instance == null)
                    instance = new UserMgr();
            }
            return instance;
        }
    }

    private UserMgr()
    {
        userUid = 0;
        dic_userInfo = new Dictionary<int, UserInfo>();
        dic_tokenUid = new Dictionary<string, int>();
    }

    public int UserLogin(string _token, string _socketIp)
    {
        int _uid;
        if (dic_tokenUid.ContainsKey(_token))
        {
            _uid = dic_tokenUid[_token];
        }
        else
        {
            userUid++;
            _uid = userUid;
            dic_tokenUid[_token] = userUid;
        }

        dic_userInfo[_uid] = new UserInfo(_socketIp, true);

        return _uid;
    }

    public void UserLogout(string _socketIp)
    { 
        //  掉线
    }

    public void UserLogout(int _uid)
    { 
        //  自己登出
    }

    public UserInfo GetUserInfo(int _uid)
    {
        return dic_userInfo[_uid];
    }
}
