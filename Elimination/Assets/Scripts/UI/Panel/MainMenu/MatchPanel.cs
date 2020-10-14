using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Protocol protocol = new Protocol(new MatchMessage());

        Debug.Log("发送消息" + protocol.ToString());
        ClientTcp.Instance.Send(protocol, typeof(MatchMessage).ToString(), OnMatchBack);
    }

    private void OnMatchBack(Protocol protocol)
    {
        MatchMessage mm = protocol.Decode<MatchMessage>();
        Debug.Log("返回匹配信息");
    }

    #endregion
}
