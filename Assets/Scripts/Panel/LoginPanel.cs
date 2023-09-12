using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using PlayerMsg;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginPanel : BasePanel
{
    //账号输入框
    private TMP_InputField _idInput;
    //密码输入框
    private TMP_InputField _pwInput;
    //登录按钮
    private Button _loginBtn;
    //注册按钮
    private Button _regBtn;
    //初始化
    public override void OnInit()
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        _idInput = skin.transform.Find("IdInput").GetComponent<TMP_InputField>();
        _pwInput = skin.transform.Find("PwInput").GetComponent<TMP_InputField>();
        _loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        _regBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        //添加按钮监听
        _loginBtn.onClick.AddListener(OnloginClick);
        _regBtn.onClick.AddListener(OnRegClick);
        //网络协议监听
        NetManager.AddMsgListener("S2C_Login", OnMsgLogin);
        NetManager.AddMsgListener("S2C_Register_Accoount", OnMsgRegister);
        //网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        //连接到服务器
        NetManager.Connect("127.0.0.1", 8888);
    }
    //关闭
    public override void OnClose()
    {
        //网络协议监听
        NetManager.RemoveMsgListener("S2C_Login", OnMsgLogin);
        //网络事件监听
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }
    //连接成功回调
    void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
    }
    //连接失败回调
    void OnConnectFail(string err)
    {
        PanelManager.Open<TipPanel>(err);
    }
    //当按下登录按钮
    public void OnloginClick()
    {
        Debug.Log("OnloginClick");
        //用户名密码为空
        if (_idInput.text == "" || _pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        //发送
        C2S_Login msgLogin = new C2S_Login()
        {
            UserName = _idInput.text,
            Password = _pwInput.text
        };
        byte[] msgBytes = msgLogin.ToByteArray();
        NetManager.Send(MessageId.MessageId.C2SLogin, msgBytes);
    }
    //当按下注册按钮
    public void OnRegClick()
    {
        PanelManager.Open<RegisterPanel>();
    }
    
    //收到登录协议
    public void OnMsgLogin(byte[] msgData)
    {
        S2C_Login msgLogin = S2C_Login.Parser.ParseFrom(msgData);
        if (msgLogin.Ok == 0)
        {
            //设置玩家Id
            GameMain.PlayerId = msgLogin.PlayerId.ToString();
            Debug.Log("登录成功");
            Close();
            SceneManager.LoadScene("move");
            //打开聊天面板
            PanelManager.Open<ChatPanel>();
        }
        else
        {
            PanelManager.Open<TipPanel>("登录失败");
        }
    }
    //收到注册协议
    public void OnMsgRegister(byte[] msgData)
    {
        S2C_Register_Accoount msgRegister = S2C_Register_Accoount.Parser.ParseFrom(msgData);
        if (msgRegister.RetCode == 0)
        {
            //设置玩家Id
            GameMain.PlayerId = msgRegister.PlayerId;
            Debug.Log("创建成功");
            Close();
        }
        else if(msgRegister.RetCode == 1)
        {
            PanelManager.Open<TipPanel>("抱歉，这个账号已经存在");
        }
        else
        {
            PanelManager.Open<TipPanel>("服务器错误");
        }
    }
}
