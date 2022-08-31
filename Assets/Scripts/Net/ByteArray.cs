using System;

public class ByteArray
{
    //默认大小
    const int DEFAULT_SIZE = 1024;
    //初始大小
    int initSize = 0;
    //缓冲区
    public byte[] bytes;
    //读写位置
    public int readIdx = 0;
    public int writeIdx = 0;
    //容量
    private int capacity = 0;
    //剩余空间
    public int remain { get { return capacity - writeIdx; } }
    //数据长度
    public int length { get { return writeIdx - readIdx; } }
    //构造函数
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }
    //构造函数
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }
    //重设尺寸
    public void ReSize(int size)
    {
        if (size < length) return;
        if (size < initSize) return;
        int n = 1;
        while (n < size) n *= 2;
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;
    }
    //检查并且移动数据
    public void CheckAndMoveBytes()
    {
        if (length < 8)
        {
          MoveBytes();
        }
    }
    //移动数据
    public void MoveBytes()
    {
        Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;
    }
    //写入数据
    public int Write(byte[] bs,int offset,int count)
    {
        if (remain < count)
        {
            ReSize(length + count);
        }
        Array.Copy(bytes, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }
    //读取数据
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, length);
        Array.Copy(bytes, readIdx, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }
    //读取Int16
    public UInt16 ReadLittleEndianInt16()
    {
        if (length < 2) return 0;
        UInt16 ret = (UInt16)((bytes[1] << 8) | bytes[0]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }
    //读取Int32
    public UInt32 ReadLittleEndianInt32()
    {
        if (length < 4) return 0;
        UInt32 ret = (UInt32)(
            (bytes[readIdx + 3] << 24)|
            (bytes[readIdx + 2] << 16)|
            (bytes[readIdx + 1] << 8) |
            bytes[readIdx + 0]);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }
    //读取Int64
    public UInt64 ReadLittleEndianInt64()
    {
        if (length < 8) return 0;
        UInt64 ret = (UInt64)(
            (bytes[readIdx + 7] << 56) |
            (bytes[readIdx + 6] << 48) |
            (bytes[readIdx + 5] << 40) |
            (bytes[readIdx + 4] << 32) |
            (bytes[readIdx + 3] << 24) |
            (bytes[readIdx + 2] << 16) |
            (bytes[readIdx + 1] << 8) |
            bytes[0]);
        readIdx += 8;
        CheckAndMoveBytes();
        return ret;
    }

    public UInt16 ReadBigEndianInt16()
    {
        if (length < 2) return 0;
        UInt16 ret = (UInt16)(bytes[readIdx + 1] | (bytes[readIdx + 0] << 8));
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }
    //读取Int32
    public UInt32 ReadBigEndianInt32()
    {
        if (length < 4) return 0;
        UInt32 ret = (UInt32)(
            (bytes[readIdx + 3] ) |
            (bytes[readIdx + 2] << 8) |
            (bytes[readIdx + 1] << 16) |
            bytes[readIdx + 0] << 24);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }
    //读取Int64
    public UInt64 ReadBigEndianInt64()
    {
        if (length < 8) return 0;
        UInt64 ret = (UInt64)(
            (bytes[readIdx + 7] ) |
            (bytes[readIdx + 6] << 8 ) |
            (bytes[readIdx + 5] << 16 ) |
            (bytes[readIdx + 4] << 24 ) |
            (bytes[readIdx + 3] << 32) |
            (bytes[readIdx + 2] << 40) |
            (bytes[readIdx + 1] << 48) |
            bytes[readIdx + 0] << 56);
        readIdx += 8;
        CheckAndMoveBytes();
        return ret;
    }

    public void WriteLittleEndianUInt(Byte[] b, UInt64 v)
    {
        b[writeIdx + 7] = (byte)(v >> 56);
        b[writeIdx + 6] = (byte)(v >> 48);
        b[writeIdx + 5] = (byte)(v >> 40);
        b[writeIdx + 4] = (byte)(v >> 32);
        b[writeIdx + 3] = (byte)(v >> 24);
        b[writeIdx + 2] = (byte)(v >> 16);
        b[writeIdx + 1] = (byte)(v >> 8);
        b[writeIdx + 0] = (byte)v;
    }
    public void WriteLittleEndianUInt(Byte[] b, UInt32 v)
    {
        b[writeIdx + 3] = (byte)(v >> 24);
        b[writeIdx + 2] = (byte)(v >> 16);
        b[writeIdx + 1] = (byte)(v >> 8);
        b[writeIdx + 0] = (byte)v;
    }
    public void WriteLittleEndianUInt(Byte[] b, UInt16 v)
    {
        b[1] = (byte)(v >> 8);
        b[0] = (byte)v;
    }
    public void WriteBigEndianUInt(Byte[] b, UInt64 v)
    {
        b[writeIdx + 0] = (byte)(v >> 56);
        b[writeIdx + 1] = (byte)(v >> 48);
        b[writeIdx + 2] = (byte)(v >> 40);
        b[writeIdx + 3] = (byte)(v >> 32);
        b[writeIdx + 4] = (byte)(v >> 24);
        b[writeIdx + 5] = (byte)(v >> 16);
        b[writeIdx + 6] = (byte)(v >> 8);
        b[writeIdx + 7] = (byte)v;
    }
    public void WriteBigEndianUInt(Byte[] b, UInt32 v)
    {
        b[writeIdx + 0] = (byte)(v >> 24);
        b[writeIdx + 1] = (byte)(v >> 16);
        b[writeIdx + 2] = (byte)(v >> 8);
        b[writeIdx + 3] = (byte)v;
    }
    public void WriteBigEndianUInt(Byte[] b, UInt16 v)
    {
        b[writeIdx + 0] = (byte)(v >> 8);
        b[writeIdx + 1] = (byte)v;
    }
}
