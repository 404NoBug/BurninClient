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
    //�����׽���
    static Socket socket;
    //���ջ�����
    static ByteArray readBuff = new ByteArray();
    //ί������
    public delegate void MsgListener(byte[] msgData);
    //�����б�
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //��Ϣ�б�
    static List<Message> msgList = new List<Message>();
    //����
    static Queue<ByteArray> writeQueue = new Queue<ByteArray>();
    //��Ӽ���
    public static void AddListener(string msgName, MsgListener msgListener)
    {
        listeners[msgName] = msgListener;
    }
    //��ȡ����
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }
    //����
    public static void Connect(string ip, int port)
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect (��ͬ�������򻯴���)
        socket.Connect(ip, port);
        //BeginReceive
        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCalback, socket);
    }
    //Receive�ص�
    private static void ReceiveCalback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //��ȡ�������ݳ���
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;
            //�����������Ϣ
            UnPack();
            //�ȴ� ģ��ճ��
            //System.Threading.Thread.Sleep(1000 * 30);
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCalback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }
    //����
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
    //Send�ص�
    public static void SendCallback(IAsyncResult ar)
    {
        //��ȡstate
        Socket socket = (Socket)ar.AsyncState;
        //EndSend�Ĵ���
        int count = socket.EndSend(ar);
        //�ж��Ƿ������
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIdx += count;
        if (ba.length == count)//��������
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
        else if (ba != null) //���Ͳ����������������Ҵ��ڵڶ������� //��������
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
    }
    //���Э������
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
    //���Э������
    private static void UnPack()
    {
        //��Ϣ����
        if (readBuff.writeIdx <= 8) return;
        int bodyLength = (int)readBuff.ReadBigEndianInt64();
        //��Ϣ��
        if (readBuff.length < bodyLength - readBuff.readIdx) return;
        //��Ϣ����
        UInt64 msgId = readBuff.ReadBigEndianInt64();
        byte[] msgDataByte = new byte[bodyLength - 8 - 8];
        readBuff.Read(msgDataByte, 0, bodyLength - 8 - 8);
        Message message;
        message.ID = msgId;
        message.Data = msgDataByte;
        msgList.Add(message);
        //������ȡ��Ϣ
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
