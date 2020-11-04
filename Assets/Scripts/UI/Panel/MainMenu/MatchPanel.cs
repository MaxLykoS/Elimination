using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchPanel : PanelBase
{
    private Button btnMatch;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);

        skinPath = "Panel/MainMenu/MatchPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        btnMatch = skinTrans.Find("BtnMatch").GetComponent<Button>();

        btnMatch.onClick.AddListener(OnMatchClick);
    }
    #endregion

    #region 按钮监听

    public void OnMatchClick()
    {
        //发送登录申请
        Protocol protocol = new Protocol(new TcpMatchRequestMessage(ClientGlobal.UID));

        Debug.Log("发送消息" + protocol.ToString());
        ClientTcp.Instance.Send(protocol, typeof(TcpEnterBattleMessage).ToString(), OnMatchBack);
        Debug.Log("进入匹配队列");
        //  进入匹配队列动画
        //  ......
        btnMatch.onClick.RemoveAllListeners();
    }

    private void OnMatchBack(Protocol protocol)
    {
        TcpEnterBattleMessage mm = protocol.Decode<TcpEnterBattleMessage>();
        Debug.Log("接收服务器发送的匹配完成信息" + mm.ToString());
        //  根据服务器发来的匹配信息，进入游戏
        //  ......
        ClientGlobal.Instance.AddAction(() =>{
            EnterBattle(mm);
        });
    }

    private void EnterBattle(TcpEnterBattleMessage bm)
    {
        Debug.Log("进入战场");
        BattleData.Instance.UpdateBattleInfo(bm.Seed, bm.BattleUserInfos);
        //  LoadScene 战场场景
        SceneManager.LoadSceneAsync(1);
    }
    #endregion
}
