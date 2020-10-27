using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData 
{
    public static int RandSeed;  //    随机种子
    public static int BattleID;

    public List<BattleUserInfo> list_battleUser;

    private int curOperationID;
    public PlayerOperation SelfOperation;

    private int curFrameID;
    private int maxFrameID;
    private int maxSendNum;

    private List<int> lackFrame;
    private Dictionary<int, AllPlayerOperation> dic_frameData;

    private static BattleData instance;
    public static BattleData Instance
    {
        get
        {
            if (instance == null)
                instance = new BattleData();
            return instance;
        }
    }

    private BattleData()
    {
        curOperationID = 1;
        SelfOperation = new PlayerOperation();

        curFrameID = 0;
        maxFrameID = 0;
        maxSendNum = 5;

        lackFrame = new List<int>();
        dic_frameData = new Dictionary<int, AllPlayerOperation>();
    }

    public void UpdateBattleInfo(int _randSeed, List<BattleUserInfo> _userInfo)
    {
        //Debug.Log("更新战场信息");
        RandSeed = _randSeed;
        list_battleUser = new List<BattleUserInfo>(_userInfo);

        foreach (BattleUserInfo info in list_battleUser)
        {
            if (info.Uid == ClientGlobal.UID)
            {
                BattleID = info.BattleID;
                Debug.Log("自己的战斗ID:" + BattleID);
            }
        }
    }

    public int GetFrameDataNum()
    {
        if (dic_frameData == null)
            return 0;
        else
            return dic_frameData.Count;
    }

    public bool TryGetNextPlayerOp(out AllPlayerOperation _op)
    {
        int _frameID = curFrameID + 1;
        return dic_frameData.TryGetValue(_frameID, out _op);
    }

    public void RunOpSucces()
    {
        curFrameID++;
    }

    public void AddNewFrameData(int _frameID, AllPlayerOperation _ops)
    {
        dic_frameData[_frameID] = _ops;
        for (int i = maxFrameID + 1; i < _frameID; i++)
        {
            lackFrame.Add(i);
            Debug.Log("缺失：" + i);
        }
        maxFrameID = _frameID;

        //  发送缺失帧数据
        //  只发送所有缺失帧的前5个
        if (lackFrame.Count > 0)
        {
            if (lackFrame.Count > maxSendNum)
            {
                List<int> sendList = lackFrame.GetRange(0, maxSendNum);
                BattleConn.Instance.SendDeltaFrames(SelfOperation.BattleID, sendList);
            }
            else
            {
                BattleConn.Instance.SendDeltaFrames(SelfOperation.BattleID, lackFrame);
            }
        }
    }

    public void ClearData()
    {
        curOperationID = 1;
        SelfOperation.Move = 121;
        curFrameID = 0;
        maxFrameID = 0;
        maxSendNum = 5;

        lackFrame.Clear();
        dic_frameData.Clear();
    }

    public void AddLackFrameData(int _frameID, AllPlayerOperation _newOps)
    {
        //  删除缺失的帧记录
        if (lackFrame.Contains(_frameID))
        {
            dic_frameData[_frameID] = _newOps;
            lackFrame.Remove(_frameID);
            Debug.Log("补上:" + _frameID);
        }
    }

    public void UpdateKeyInput(char key)
    {
        SelfOperation.Keyinput = key.ToString();
    }

    public void ResetSelfOperation()
    {
        SelfOperation.Move = 121;
        SelfOperation.Keyinput = "";
    }
}
