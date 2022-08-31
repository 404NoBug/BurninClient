using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMsg;
using Google.Protobuf;
using System;

public class PlayerCreat : MonoBehaviour
{

    //人物模型预设
    private GameObject humanPrefab;
    //人物列表
    private BaseHuman myHuman;
    private Dictionary<string, BaseHuman> otherHumans;
    public void OnEnterMap()
    {
        //添加一个角色
        GameObject ojb = (GameObject)Instantiate(humanPrefab);
        float x = UnityEngine.Random.Range(-5, 5);
        float z = UnityEngine.Random.Range(105, 115);
        ojb.transform.position = new Vector3(x, 0, z);
        myHuman = ojb.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        //发送协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        pos_info pos_Info = new pos_info()
        {
            X = pos.x ,
            Y = pos.z
        };
        C2GS_EnterSence enterMsg = new C2GS_EnterSence()
        {
            UId = NetManager.GetDesc(),
            Pos = pos_Info,
            Dir = eul.y
        };
        byte[] msgBytes = enterMsg.ToByteArray();
        NetManager.Send(MessageId.MessageId.C2GsEnterSence, msgBytes);
    }

    void Player_OnEnter(Byte[] msgData)
    {
        Debug.Log("Player_OnEnter");
        GS2C_EnterSence msg_enter = GS2C_EnterSence.Parser.ParseFrom(msgData);
        if (otherHumans == null)
        {
            otherHumans = new Dictionary<string, BaseHuman>();
        }
        //是自己
        if (msg_enter.UId == NetManager.GetDesc() || otherHumans.ContainsKey(msg_enter.UId))
            return;
        //添加一个角色
        GameObject ojb = (GameObject)Instantiate(humanPrefab);
        ojb.transform.position = new Vector3(msg_enter.Pos.X, 0, msg_enter.Pos.Y);
        ojb.transform.eulerAngles = new Vector3(0, msg_enter.Dir, 0);
        BaseHuman h = ojb.AddComponent<SyncHuman>();
        h.desc = msg_enter.UId;
        otherHumans.Add(msg_enter.UId, h);
    }

    void PlayerMove(Byte[] msgData)
    {
        Debug.Log("PlayerMove");
        GS2C_PlayerMove msg_move = GS2C_PlayerMove.Parser.ParseFrom(msgData);
        //移动
        if (!otherHumans.ContainsKey(msg_move.UId)) return;
        BaseHuman baseHuman = otherHumans[msg_move.UId];
        Vector3 vector3 = new Vector3(msg_move.Pos.X, 0, msg_move.Pos.Y);
        baseHuman.MoveTo(vector3);
    }

    void PlayerStopMove(Byte[] msgData)
    {
        Debug.Log("PlayerStopMove");
        GS2C_PlayerStopMove msg_stopMove = GS2C_PlayerStopMove.Parser.ParseFrom(msgData);
        //移动
        if (!otherHumans.ContainsKey(msg_stopMove.UId)) return;
        BaseHuman baseHuman = otherHumans[msg_stopMove.UId];
        baseHuman.StopMove(msg_stopMove.Dir);
    }

    void OtherPlayer_LeaveMap(Byte[] msgData)
    {
        Debug.Log("OtherPlayer_LeaveMap");
        GS2C_PlayerLeave msg_enter = GS2C_PlayerLeave.Parser.ParseFrom(msgData);
        if (!otherHumans.ContainsKey(msg_enter.UId)) return;
        BaseHuman baseHuman = otherHumans[msg_enter.UId];
        Destroy(baseHuman.gameObject);
        otherHumans.Remove(msg_enter.UId);
    }

    void OtherPlayer_OnEnter(Byte[] msgData)
    {
        Debug.Log("OtherPlayer_OnEnter");
        GS2C_ONLinePlayerList msg_enter = GS2C_ONLinePlayerList.Parser.ParseFrom(msgData);
        if (otherHumans == null)
        {
            otherHumans = new Dictionary<string, BaseHuman>();
        }
        for (int i = 0; i < msg_enter.List.Count; i++)
        {
            //是自己
            if (msg_enter.List[i].UId == NetManager.GetDesc() || otherHumans.ContainsKey(msg_enter.List[i].UId))
                continue;
            //添加一个角色
            GameObject ojb = (GameObject)Instantiate(humanPrefab);
            ojb.transform.position = new Vector3(msg_enter.List[i].Pos.X, 0, msg_enter.List[i].Pos.Y);
            ojb.transform.eulerAngles = new Vector3(0, msg_enter.List[i].Dir, 0);
            BaseHuman h = ojb.AddComponent<SyncHuman>();
            h.desc = msg_enter.List[i].UId;
            otherHumans.Add(msg_enter.List[i].UId, h);
            Debug.Log("OnEnter");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddListener("GS2C_EnterSence", Player_OnEnter);
        NetManager.AddListener("GS2C_ONLinePlayerList", OtherPlayer_OnEnter);
        NetManager.AddListener("GS2C_PlayerLeave", OtherPlayer_LeaveMap);
        NetManager.AddListener("GS2C_PlayerMove", PlayerMove);
        NetManager.AddListener("GS2C_PlayerStopMove", PlayerStopMove);
        string fname = "Prefabs/Ethan";
        humanPrefab = Resources.Load<GameObject>(fname);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
