using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tip
{
    public class WarningTip : PanelBase
    {
        private Text infoText;
        private Button ackButton;

        private string str = "";

        #region 声明周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Tip/General/WarningTip";
            layer = PanelLayer.Tip;

            //要显示的字符为args[1]
            if (args.Length == 1)
            {
                str = (string)args[0];
            }
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            infoText = skinTrans.Find("Image/TextInfo").GetComponent<Text>();
            ackButton = skinTrans.Find("Image/BtnAck").GetComponent<Button>();

            infoText.text = str;
            ackButton.onClick.AddListener(OnBtnClick);
        }
        #endregion

        #region 按钮监听
        private void OnBtnClick()
        {
            Close();
        }
        #endregion
    }
}