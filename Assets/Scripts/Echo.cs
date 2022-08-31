using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using TMPro;
using System;

public class Echo : MonoBehaviour
{
    //�����׽���
    Socket socket;
    //UI
    public TMP_InputField InputField;
    public TextMeshProUGUI text;
    //���ջ�����
    byte[] readBuff = new byte[1024];
    //������Ӱ�ť
    public void Connection()
    {
        //Scoket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connet
        socket.BeginConnect("127.0.0.1", 8888,ConnectCallback, socket);
    }

    //Connect�ص�
    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Success");
            //socket.BeginReceive(readBuff, 0, 1024 ,0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
        }
    }
    //Receive�ص�
 /*   public void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndReceive(ar);
            recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }*/
    //������Ͱ�ť
    public void Send()
    {
        //Send
        if (socket == null)
        {
            return;
        }
        if (socket.Poll(0, SelectMode.SelectWrite))
        {
            string sendStr = InputField.text;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            socket.Send(sendBytes);
        }
        //socket.BeginSend(sendBytes, 0, sendBytes.Length - 1, 0, SendCallback, socket);
        //����ҪReceive��
    }
    //Send�ص�
    public void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket Send succ" + count);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Send fail" + ex.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(socket == null)
        {
            return;
        }
        if (socket.Poll(0,SelectMode.SelectRead))
        {
            int count = socket.Receive(readBuff);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            text.text = recvStr;
        }
    }
}
