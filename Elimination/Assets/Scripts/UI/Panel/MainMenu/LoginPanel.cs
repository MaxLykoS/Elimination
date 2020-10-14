using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Tip;
using System.Net.Sockets;

namespace UI.Panel
{
    public class LoginPanel : PanelBase
    {
        private Button btnLogin;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/MainMenu/LoginPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnLogin = skinTrans.Find("BtnLogin").GetComponent<Button>();

            btnLogin.onClick.AddListener(OnLoginClick);
        }
        #endregion

        #region 按钮监听

        public void OnLoginClick()
        {
            string host = "127.0.0.1";
            int port = 1234;
            ClientTcp.Instance.Connect(host, port);

            //发送登录申请
            Protocol protocol = new Protocol(new LoginMessage("MaxLykoS","123456"));

            Debug.Log("发送消息" + protocol.ToString());
            ClientTcp.Instance.Send(protocol,typeof(LoginFeedbackMessage).ToString(), OnLoginBack);
        }

        private void OnLoginBack(Protocol protocol)
        {
            LoginFeedbackMessage fb = protocol.Decode<LoginFeedbackMessage>();
            if (fb.Status == LoginFeedbackMessage.LoginStatus.Success)
            {
                Debug.Log("登录成功" + fb.ToString());
            }
            else
            {
                Debug.Log("登录失败");
            }
        }
        #endregion
    }
}