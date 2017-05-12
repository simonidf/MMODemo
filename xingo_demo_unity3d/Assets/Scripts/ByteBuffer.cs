using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class ByteBuffer {

    MemoryStream stream = null;
    BinaryWriter writer = null;
    BinaryReader reader = null;

    public ByteBuffer()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public ByteBuffer(byte[] data)
    {
        if (data != null)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
        }
        else
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    public void Close()
    {
        if (writer != null) writer.Close();
        if (reader != null) reader.Close();

        stream.Close();
        writer = null;
        reader = null;
        stream = null;
    }

    public void WriteByte(byte v)
    {
        writer.Write(v);
    }

    public void WriteInt(int v)
    {
        writer.Write((int)v);
    }

    public void WriteUInt32(UInt32 v)
    {
        writer.Write(((UInt32)v));
    }

    public void WriteShort(ushort v)
    {
        writer.Write((ushort)v);
    }

    public void WriteLong(long v)
    {
        writer.Write((long)v);
    }

    public void WriteFloat(float v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToSingle(temp, 0));
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }

    public void WriteString(string v)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(v);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    public void WriteBytes(byte[] v)
    {
        writer.Write(v);
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public UInt32 ReadUInt32()
    {
        return (UInt32)reader.ReadUInt32();
    }

    public int ReadInt()
    {
        return (int)reader.ReadInt32();
    }

    public ushort ReadShort()
    {
        return (ushort)reader.ReadInt16();
    }

    public long ReadLong()
    {
        return (long)reader.ReadInt64();
    }

    public float ReadFloat()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
        Array.Reverse(temp);
        return BitConverter.ToSingle(temp, 0);
    }

    public double ReadDouble()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
        Array.Reverse(temp);
        return BitConverter.ToDouble(temp, 0);
    }

    public string ReadString()
    {
        ushort len = ReadShort();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte[] ReadBytes(int len)
    {
        return reader.ReadBytes(len);
    }

    public byte[] ToBytes()
    {

        writer.Flush();

        var result = stream.ToArray();

        return result;
    }

    public void Flush()
    {
        writer.Flush();
    }
}