using System.Collections;
using System.Collections.Generic;
using UI.Panel;
using UnityEngine;

public class ClientEntrance : MonoBehaviour
{
    ClientTcp client = new ClientTcp();

    void Start()
    {
        PanelMgr.Instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        client.Update();
    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
