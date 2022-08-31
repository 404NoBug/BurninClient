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
        NetManager.AddListener("GS2C_PlayerMove", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
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
