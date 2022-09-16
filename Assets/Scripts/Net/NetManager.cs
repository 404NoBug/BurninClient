using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Linq;
using PlayerMsg;
using Google.Protobuf;

// struct Message
// {
//     public UInt64 ID;
//     public byte[] Data;
// };

public static class NetManager
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff = new ByteArray();
    //委托类型
    public delegate void MsgListener(byte[] msgData);
    //监听列表
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<MsgBase> msgList = new List<MsgBase>();
    //消息列表长度
    private static int msgCount = 0;
    //每一次Update处理的消息数量
    private readonly static int MAX_MESSAGE_FIRE = 10;
    //定义
    static Queue<ByteArray> writeQueue = new Queue<ByteArray>();
    //是否关闭连接
    static bool isClosing = false;
    //是否正在连接
    static bool isClonnecting = false;
    //是否启用心跳
    public static bool isUsePing = true;
    //心跳间隔
    public static int pingInterval = 30;
    //上一次发送PING的时间
    static float lastPingTime = 0;
    //上一次收到PONG的时间
    static float lastPongTime = 0;
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    //事件委托类型
    public delegate void EventListener(String err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
    //初始化状态
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //消息列表
        msgList = new List<MsgBase>();
        //消息列表长度
        msgCount = 0;
        //是否正在连接
        isClonnecting = false;
        //是否正在关闭
        isClosing = false;
        //上一次发送PING的时间
        lastPingTime = Time.time;
        //上一次收到PONG的时间
        lastPongTime = Time.time;
        //监听PONG协议
        if (!msgListeners.ContainsKey("C2GS_MsgPong"))
        {
            AddMsgListener("C2GS_MsgPong", OnMsgPong);
        }
    }
    //监听PONG协议
    private static void OnMsgPong(byte[] msgData)
    {
        Debug.Log("OnMsgPong lastPongTime = " + Time.time);
        lastPongTime = Time.time;
    }
    
    //发送PING协议
    private static void PingUpdate()
    {
        //是否启用
        if (!isUsePing)
        {
            return;
        }
        //发送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            C2GS_MsgPing pingMsg = new C2GS_MsgPing(){};
            byte[] msgBytes = pingMsg.ToByteArray();
            Send(MessageId.MessageId.C2GsMsgPing, msgBytes);
        }
        //检测PONG时间
        if (Time.time - lastPongTime > pingInterval*4)
        {
            //与服务器断连
            Close();
        }
    }
    //添加消息监听
    public static void AddMsgListener(string msgName, MsgListener msgListener)
    {
        //添加
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += msgListener;
        }
        //新增
        else
        {
            msgListeners[msgName] = msgListener;
        }
    }
    //删除消息监听
    public static void RemoveMsgListener(string msgName, MsgListener msgListener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= msgListener;
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }
    //分发消息
    private static void FireMsg(string msgName, byte[] msgData)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgData);
        }
    }
    //添加事件监听
    public static void AddEventListener(NetEvent netEvent, EventListener eventListener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += eventListener;
        }
        //新增加事件
        else{
            eventListeners[netEvent] = eventListener;
        }
    }
    //删除监听事件
    public static void RemoveEventListener(NetEvent netEvent, EventListener eventListener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= eventListener;
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }
    //分发事件
    private static void FireEvent(NetEvent netEvent, String err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }
    //获取描述
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }
    //连接
    public static void Connect(string ip, int port)
    {
        //判断状态
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isClonnecting)
        {
            Debug.Log("Connect fail, isClonnecting");
            return;
        }
        //初始化成员
        InitState();
        //设置参数
        socket.NoDelay = true;
        //Connect (用同步方法简化代码)
        //socket.Connect(ip, port);
        //Connect
        isClonnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }
    //Connect回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc,"");
            isClonnecting = false;
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCalback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect Fail " + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isClonnecting = false;
        }
    }
    //Receive回调
    private static void ReceiveCalback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            //处理二进制消息
            UnPack();
            //继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length);
            }
            //等待 模拟粘包
            //System.Threading.Thread.Sleep(1000 * 30);
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCalback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }
    //发送
    public static void Send(MessageId.MessageId MsgId, byte[] MsgData)
    {
        //判断状态
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isClonnecting)
        {
            return;
        }
        if (isClosing)
        {
            return;
        }
        //打包协议
        byte[] sendBytes = Pack(MsgId, MsgData);
        //写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count;//writeQueue的长度
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        //send
        if (count == 1)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        //socket.Send(sendBytes);
    }
    //Send回调
    public static void SendCallback(IAsyncResult ar)
    {
        //获取state、EndSend的处理
        Socket socket = (Socket)ar.AsyncState;
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }
        
        //EndSend的处理
        int count = socket.EndSend(ar);
        //判断是否发送完成
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIdx += count;
        if (ba.length == 0)//发送完整
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                if (writeQueue.Count != 0)
                {
                    ba = writeQueue.First();
                }
            }
        }
        if (ba != null) //发送不完整，或发送完整且存在第二条数据 //继续发送
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        else if (isClosing)
        {
            socket.Close();
        }
    }
    //打包协议数据
    private static byte[] Pack(MessageId.MessageId MsgId, byte[] MsgData)
    {
        byte[] sendBytes = new byte[8 + 8 + MsgData.Length];
        short sendBytesSort = System.Net.IPAddress.HostToNetworkOrder((short)sendBytes.Length);
        byte[] MsgLength = BitConverter.GetBytes(sendBytesSort);
        int MsgIdByteSort = System.Net.IPAddress.HostToNetworkOrder((int)MsgId);
        byte[] MsgIdByte = BitConverter.GetBytes(MsgIdByteSort);
        Buffer.BlockCopy(MsgLength, 0, sendBytes, 8 - MsgLength.Length, MsgLength.Length);
        Buffer.BlockCopy(MsgIdByte, 0, sendBytes, 16 - MsgIdByte.Length, MsgIdByte.Length);
        Buffer.BlockCopy(MsgData, 0, sendBytes, 16, MsgData.Length);
        return sendBytes;
    }
    //解包协议数据
    private static void UnPack()
    {
        //消息长度
        if (readBuff.writeIdx <= 8) return;
        int bodyLength = (int)readBuff.ReadBigEndianInt64();
        //消息体
        if (readBuff.length < bodyLength - readBuff.readIdx) return;
        //消息处理
        UInt64 msgId = readBuff.ReadBigEndianInt64();
        byte[] msgDataByte = new byte[bodyLength - 8 - 8];
        readBuff.Read(msgDataByte, 0, bodyLength - 8 - 8);
        readBuff.CheckAndMoveBytes();
        MsgBase message = new MsgBase();
        message.ID = msgId;
        message.Data = msgDataByte;
        //添加到消息队列
        lock (readBuff)
        {
            msgList.Add(message);
        }
        msgCount++;
        //继续读取消息
        if (readBuff.length > 2)
        {
            UnPack();
        }
    }
    // Update is called once per frame
    public static void Update()
    {
        MsgUpdate();
        // PingUpdate();
    }
    //更新消息
    public static void MsgUpdate()
    {
        //初步判断，提升效率
        if (msgCount == 0)
        {
            return;
        }
        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //获取第一条消息
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (msgBase != null)
            {
                string MessageEnumName = EnumTools.GetMessageIdEnumNameByKey(msgBase.ID);
                if (Enum.TryParse(MessageEnumName, out MessageId.MessageId bar))
                {
                    string MsgNameStr = bar.ToDescription();
                    FireMsg(MsgNameStr, msgBase.Data);
                }
            }
            //没消息了
            else
            {
                break;
            }
        }
    }

    public static void OnDestroy()
    {

    }
    //关闭连接
    public static void Close()
    {
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }

        if (isClonnecting)
        {
            return;
        }
        //还有数据在发送
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else //没有数据直接关闭
        {
            socket.Close();
            FireEvent(NetEvent.Close,"");
        }
    }
}