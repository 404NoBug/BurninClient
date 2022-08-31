using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using MessageId;
using System.Linq;

struct Message
{
    public UInt64 ID;
    public byte[] Data;
};

public class NetManager : MonoBehaviour
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff = new ByteArray();
    //委托类型
    public delegate void MsgListener(byte[] msgData);
    //监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<Message> msgList = new List<Message>();
    //定义
    static Queue<ByteArray> writeQueue = new Queue<ByteArray>();
    //添加监听
    public static void AddListener(string msgName, MsgListener msgListener)
    {
        listeners[msgName] = msgListener;
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
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect (用同步方法简化代码)
        socket.Connect(ip, port);
        //BeginReceive
        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCalback, socket);
    }
    //Receive回调
    private static void ReceiveCalback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;
            //处理二进制消息
            UnPack();
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
        if (socket == null) return;
        if (!socket.Connected) return;
        byte[] sendBytes = Pack(MsgId, MsgData);
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        if (count == 1)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        //socket.Send(sendBytes);
    }
    //Send回调
    public static void SendCallback(IAsyncResult ar)
    {
        //获取state
        Socket socket = (Socket)ar.AsyncState;
        //EndSend的处理
        int count = socket.EndSend(ar);
        //判断是否发送完成
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIdx += count;
        if (ba.length == count)//发送完整
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
        else if (ba != null) //发送不完整，或发送完整且存在第二条数据 //继续发送
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
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
        Message message;
        message.ID = msgId;
        message.Data = msgDataByte;
        msgList.Add(message);
        //继续读取消息
        if (readBuff.length > 2)
        {
            UnPack();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public static void Update()
    {
        if (msgList.Count <= 0) return;
        Message message = msgList[0];
        msgList.RemoveAt(0);
        string MessageEnumName = EnumTools.GetMessageIdEnumNameByKey(message.ID);
        if (Enum.TryParse(MessageEnumName, out MessageId.MessageId bar))
        {
            string MsgNameStr = bar.ToDescription();
            if (listeners.ContainsKey(MsgNameStr))
            {
                listeners[MsgNameStr](message.Data);
            }
        };
        
    }

    public static void OnDestroy()
    {
        socket.Close();
    }
}
