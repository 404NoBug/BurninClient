using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using PlayerMsg;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    //账号输入框
    private TMP_InputField _idInput;
    //密码输入框
    private TMP_InputField _pwInput;
    //重复密码输入框
    private TMP_InputField _repInput;
    //注册按钮
    private Button _regBtn;
    //关闭按钮
    private Button _closeBtn;
    //初始化
    public override void OnInit()
    {
        skinPath = "RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        _idInput = skin.transform.Find("IdInput").GetComponent<TMP_InputField>();
        _pwInput = skin.transform.Find("PwInput").GetComponent<TMP_InputField>();
        _repInput = skin.transform.Find("RepInput").GetComponent<TMP_InputField>();
        _regBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //添加按钮监听
        _regBtn.onClick.AddListener(OnRegClick);
        _closeBtn.onClick.AddListener(OnCloseClick);
        //网络协议监听
        NetManager.AddMsgListener("S2C_Register_Accoount", OnMsgRegister);
    }
    //关闭
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("S2C_Register_Accoount", OnMsgRegister);
    }

    public void OnMsgRegister(byte[] msgData)
    {
        S2C_Register_Accoount msgRegister = S2C_Register_Accoount.Parser.ParseFrom(msgData);
        if (msgRegister.RetCode == 0)
        {
            Debug.Log("注册成功");
            //提示
            PanelManager.Open<TipPanel>("注册成功");
            //关闭界面
            Close();
        }
        else if(msgRegister.RetCode == 1)
        {
            Debug.Log("账号存在");
            //提示
            PanelManager.Open<TipPanel>("账号存在");
        }
        else if(msgRegister.RetCode == 2)
        {
            Debug.Log("账号、密码格式错误");
            //提示
            PanelManager.Open<TipPanel>("账号、密码格式错误");
        }
        else if(msgRegister.RetCode == 3)
        { 
            Debug.Log("服务器内部错误");
            //提示
            PanelManager.Open<TipPanel>("服务器内部错误");
        }
        else
        {
            Debug.Log("msgRegister.RetCode = " + msgRegister.RetCode);
            //提示
            PanelManager.Open<TipPanel>("msgRegister.RetCode = " + msgRegister.RetCode);
        }
    }
    public void OnRegClick()
    {
        Debug.Log("OnRegClick");
        //用户名密码为空
        if (_idInput.text == "" || _pwInput.text == "" || _repInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        //两次密码不同
        if (_pwInput.text != _repInput.text)
        {
            PanelManager.Open<TipPanel>("两次输入的密码不同");
            return;
        }
        //发送
        C2S_Register_Accoount regMsg = new C2S_Register_Accoount()
        {
            UserAccoount = _idInput.text,
            Password = _pwInput.text
        };
        byte[] msgBytes = regMsg.ToByteArray();
        NetManager.Send(MessageId.MessageId.C2SRegisterAccoount, msgBytes);
    }

    public void OnCloseClick()
    {
        Close();
    }
}
