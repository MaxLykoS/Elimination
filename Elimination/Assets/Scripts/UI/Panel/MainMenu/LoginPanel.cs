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
            string host = ClientGlobal.ServerIP;
            int port = ClientGlobal.TcpServerPort;
            ClientTcp.Instance.Connect(host, port);

            //发送登录申请
            Protocol protocol = new Protocol(new TcpLoginMessage("MaxLykoS","123456",SystemInfo.deviceUniqueIdentifier));

            Debug.Log("发送消息" + protocol.ToString());
            ClientTcp.Instance.Send(protocol,typeof(TcpLoginFeedbackMessage).ToString(), OnLoginBack);
        }

        private void OnLoginBack(Protocol protocol)
        {
            TcpLoginFeedbackMessage fb = protocol.Decode<TcpLoginFeedbackMessage>();
            if (fb.Status == TcpLoginFeedbackMessage.LoginStatus.Success)
            {
                ClientGlobal.UID = fb.Uid;
                ClientGlobal.UdpSendPort = fb.UdpPort;
                Debug.Log("登录成功" + fb.ToString());
                PanelMgr.Instance.OpenPanel<MatchPanel>("");
                Close();
            }
            else
            {
                Debug.Log("登录失败");
            }
        }
        #endregion
    }
}