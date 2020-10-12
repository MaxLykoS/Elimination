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
            //string host = "127.0.0.1";
            string host = "127.0.0.1";
            int port = 1234;
            ClientTcp.Instance.proto = new Protocol();  //用来接受服务器发送的信息
            ClientTcp.Instance.Connect(host, port);

            //发送登录申请
            Protocol protocol = new Protocol(new LoginMessage("MaxLykoS","123456"));

            Debug.Log("发送消息" + protocol.ToString());
            ClientTcp.Instance.Send(protocol, OnLoginBack);
        }

        private void OnLoginBack(Protocol protocol)
        {
            LoginMessage loginMessage = protocol.Decode<LoginMessage>();
            LoginMessage.LoginStatus s = loginMessage.Status;
            if (s == LoginMessage.LoginStatus.Success)
            {
                Debug.Log("登录成功" + loginMessage.ToString());
            }
            else
            {
                Debug.Log("登录失败");
            }
        }

        #endregion
    }
}