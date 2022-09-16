using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    //玩家id
    public static string PlayerId = "";
    void Start()
    {
        //网络监听
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("S2C_Kick", OnMsgKick);
        //初始化
        PanelManager.Init();
        //打开登录面板
        PanelManager.Open<LoginPanel>();
    }
    void Update()
    {
        NetManager.Update();
    }
    //关闭连接
    void OnConnectClose(string err)
    {
        Debug.Log("断开连接");
        PanelManager.Open<TipPanel>("断开连接");
    }
    //被踢下线
    void OnMsgKick(byte[] msgData)
    {
        PanelManager.Open<TipPanel>("被踢下线");
    }
}
