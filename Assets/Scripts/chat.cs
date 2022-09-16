using PlayerMsg;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Google.Protobuf;

public class chat : MonoBehaviour
{
    public TMP_InputField InputField;
    public TMP_InputField OutField;
    private string text;
    // Start is called before the first frame update
    void Start()
    {
        text = "";
        OutField.text = text;
        NetManager.AddMsgListener("GS2C_SendChatMsg", OnChat);
    }

    void OnChat(byte[] msgData)
    {
        Debug.Log("OnChat");
        GS2C_SendChatMsg msg_chat = GS2C_SendChatMsg.Parser.ParseFrom(msgData);
        text += msg_chat.Msg.Content;
        OutField.text = text;
    }

    public void OnSend()
    {
        ChatMessage chatMessage = new ChatMessage()
        {
            Content = InputField.text
        };
        C2GS_SendChatMsg C2GS_SendChatMsg = new C2GS_SendChatMsg()
        {
            UId = NetManager.GetDesc(),
            Msg = chatMessage,
            Category = 1,
        };
        byte[] msgBytes = C2GS_SendChatMsg.ToByteArray();
        NetManager.Send(MessageId.MessageId.C2GsSendChatMsg, msgBytes);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
