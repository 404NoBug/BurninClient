using PlayerMsg;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Google.Protobuf;
using UnityEngine.UI;

public class ChatPanel  : BasePanel
{
    //聊天输入
    private TMP_InputField _inputField;
    //聊天输出
    private TMP_InputField _outField;
    //发送按钮
    private Button _sengChatBtn;
    //聊天记录
    private string text;
    //初始化
    public override void OnInit()
    {
        skinPath = "ChatPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        _inputField = skin.transform.Find("InputField ").GetComponent<TMP_InputField>();
        _outField = skin.transform.Find("OutField").GetComponent<TMP_InputField>();
        _sengChatBtn = skin.transform.Find("SendBtn").GetComponent<Button>();
        //添加按钮监听
        _sengChatBtn.onClick.AddListener(OnSendChatClick);
        //网络协议监听
        NetManager.AddMsgListener("GS2C_SendChatMsg", OnChat);
    }
    public void OnSendChatClick()
    {
        ChatMessage chatMessage = new ChatMessage()
        {
            Content = _inputField.text
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
    void OnChat(byte[] msgData)
    {
        Debug.Log("OnChat");
        GS2C_SendChatMsg msg_chat = GS2C_SendChatMsg.Parser.ParseFrom(msgData);
        text += msg_chat.Msg.Content;
        _outField.text = text;
    }
}
