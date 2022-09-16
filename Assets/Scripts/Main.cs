using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MessageId;
using Google.Protobuf;
using PlayerMsg;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddMsgListener("GS2C_PlayerMove", OnMove);
        NetManager.AddMsgListener("Leave", OnLeave);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.Connect("127.0.0.1", 8888);
    }
    //连接成功回调
    void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
        //TODO:进入游戏
    }
    //连接失败回调
    void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail " + err);
        //TODO:弹出提示框（连接失败，请重试）
    }
    //关闭网络回调
    void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose");
        //TODO:弹出提示框（网络断开）
        //TODO:弹出按钮（重新连接）
    }
    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
    void OnDestroy()
    {
        NetManager.OnDestroy();
    }
    void OnMove(Byte[] msgData)
    {
        GS2C_PlayerMove msg_move = GS2C_PlayerMove.Parser.ParseFrom(msgData);
        Debug.LogFormat("OnMove X{0}__________Y{1} ", msg_move.Pos.X, msg_move.Pos.Y);
    }
    void OnLeave(Byte[] msgData)
    {
        Debug.Log("OnLeave");
    }
}
