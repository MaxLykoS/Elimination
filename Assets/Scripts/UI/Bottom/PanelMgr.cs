using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PanelMgr : Singletion<PanelMgr>
{
    //画板
    private GameObject canvas;

    //面板
    public Dictionary<string, PanelBase> dict;

    //层级
    private Dictionary<PanelLayer, Transform> layerDict;

    //开始
    public void Awake()
    {
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
    }

    //初始化层
    private void InitLayer()
    {
        //画布
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("PanelMgr.InitLayer fail,canvas is null");

        //各个层级
        layerDict = new Dictionary<PanelLayer, Transform>();

        foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer)))
        {
            string name = pl.ToString();
            Transform transform = canvas.transform.Find(name);
            layerDict.Add(pl, transform);
        }
    }

    //打开面板
    public void OpenPanel<T>(string skinPath, params object[] args) where T : PanelBase
    {
        //已经打开
        string name = typeof(T).ToString();
        if (dict.ContainsKey(name))
            return;

        //面板脚本
        PanelBase panel = canvas.AddComponent<T>();
        panel.Init(args);
        dict.Add(name, panel);
        //加载皮肤
        skinPath = skinPath != "" ? skinPath : panel.skinPath;
        skinPath = "Prefabs/UISkin/" + skinPath;
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if (skinPath == null)
            Debug.LogError("panelMgr.OpenPanel fail,skin is null,skinPath = " + skinPath);
        panel.skin = Instantiate(skin);

        //坐标
        Transform skintrans = panel.skin.transform;
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skintrans.SetParent(parent, false); //层级

        //panel的生命周期
        panel.OnShowing();   //预留的面板动画
        //anm
        panel.OnShowed();   //加载结束时
    }

    //关闭面板
    //注意，name是该UI类的反射名，注意前面的命名空间，嵌套类记得写+号！
    public void ClosePanel(string name)
    {
        PanelBase panel;
        if (dict.ContainsKey(name))
        {
            panel = dict[name];
        }
        else
            return;

        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        Destroy(panel.skin);
        Destroy(panel);
    }
}