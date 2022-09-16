using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{
    //Layer
    public enum Layer
    {
        Panel,
        Tip,
    }

    //层级列表
    private static Dictionary<Layer, Transform> _layers = new Dictionary<Layer, Transform>();

    //面板列表
    public static Dictionary<string, BasePanel> Panels = new Dictionary<string, BasePanel>();

    //结构
    public static Transform Root;

    public static Transform Canvas;

    //初始化
    public static void Init()
    {
        Root = GameObject.Find("Root").transform;
        Canvas = Root.Find("Canvas");
        Transform panel = Canvas.Find("Panel");
        Transform tip = Canvas.Find("Tip");
        _layers.Add(Layer.Panel, panel);
        _layers.Add(Layer.Tip, tip);
    }

    //打开面板
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        //已经打开
        string name = typeof(T).ToString();
        if (Panels.ContainsKey(name))
        {
            return;
        }

        //组件
        BasePanel panel = Root.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();
        //父容器
        Transform layer = _layers[panel.layer];
        panel.skin.transform.SetParent(layer, false);
        //列表
        Panels.Add(name, panel);
        //OnShow
        panel.OnShow(para);
    }

    //关闭面板
    public static void Close(string name)
    {
        //没有打开
        if (!Panels.ContainsKey(name))
        {
            return;
        }
        BasePanel panel = Panels[name];
        //OnClose
        panel.OnClose();
        //列表
        Panels.Remove(name);
        //销毁
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);
    }
}