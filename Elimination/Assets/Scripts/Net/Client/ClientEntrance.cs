using System.Collections;
using System.Collections.Generic;
using UI.Panel;
using UnityEngine;

public class ClientEntrance : MonoBehaviour
{
    ClientTcp client = new ClientTcp();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        PanelMgr.Instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        client.Update();
        ClientGlobal.Instance.DoForAction();
    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
