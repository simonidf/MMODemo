using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using Pb;
using Google.Protobuf;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    public static bool loggedIn = false;

    // Use this for initialization
    public SocketClient()
    {
    }


    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }


    public void OnRemove()
    {
        this.Close();
        reader.Close();
        memStream.Close();
    }


    void ConnectServer(string host, int port)
    {
        client = null;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        try
        {
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Close(); Debug.LogError(e.Message);
        }
    }


    void OnConnect(IAsyncResult asr)
    {
        Debug.Log("Connect Suc");
        outStream = client.GetStream();
        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);

        Loom.QueueOnMainThread(() =>
        {
            ByteBuffer b = new ByteBuffer();

            ProtoTest.CredentialInfo p = new ProtoTest.CredentialInfo();
            p.Cre = GameObject.Find("P1").GetComponent<FlowController>().gameCredential;

            MemoryStream ms = new MemoryStream();
            p.WriteTo(ms);
            byte[] bytes = ms.ToArray();

            b.WriteInt(bytes.Length);
            b.WriteInt(5);
            b.WriteBytes(bytes);

            SendMessage(b);
        });
    }


    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msglen = (ushort)message.Length;
            //writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                //NetworkStream stream = client.GetStream(); 
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogError("client.connected----->>false");
            }
        }
    }

    void OnRead(IAsyncResult asr)
    {
        OnReceivedMessage(byteBuffer);
        lock (client.GetStream())
        {         //分析完，再次监听服务器发过来的新消息
            Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        }
    }

    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
        Debug.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }


    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
    }

    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }


    void OnRecieveMessageDeal(ByteBuffer buffer, UInt32 len = 0)
    {
        UInt32 length = 0;
        UInt32 mainId = 0;
        if (len != 0)
        {
            length = len;
        }
        else
        {
            length = buffer.ReadUInt32(); //数据字节长度
        }
        mainId = buffer.ReadUInt32(); //msgid
        byte[] b = buffer.ReadBytes((int)length); //数据字节

        NetLogic((int)mainId, b);

        int next = (int)buffer.ReadUInt32();
        if (next != 0)
        {
            OnRecieveMessageDeal(buffer, (UInt32)next);
        }
    }

    void NetLogic(int mid, byte[] bytes)
    {
        if (mid == 1)
        {
            SyncPid pid;
            pid = SyncPid.Parser.ParseFrom(bytes);

            if (NetMgr.OnBorn != null)
            {
                NetMgr.OnBorn(pid.Pid);
                GameMgr.PlayerIDS.Add(pid.Pid);
                Debug.Log("Player: " + pid);
            }
        }


        if (mid == 200)
        {

            BroadCast bc;
            bc = BroadCast.Parser.ParseFrom(bytes);
            //Debug.Log("mid 200: " + bc);

            if (bc.Tp == 1)
            {
                //talk
                if (bc.Content != null)
                {
                    TalkCenter.Content = bc.Content;
                    TalkCenter.PlayerID = bc.Pid.ToString();
                    TalkCenter.TalkFlag = true;
                }
            }
            if (bc.Tp == 2)
            {
                //有玩家加入
                if (!GameMgr.PlayerIDS.Contains(bc.Pid))
                {
                    GameMgr.BornPlayer(bc);
                    GameMgr.PlayerIDS.Add(bc.Pid);
                    Debug.Log("born player: " + bc.Pid + " pos: " + bc.P);
                }
                else
                {
                    if (NetMgr.OnMove != null)
                    {
                        NetMgr.OnMove(bc);
                    }
                }
            }

            //广播位移和旋转
            if (bc.Tp == 3)
            {
                if (NetMgr.OnMove != null)
                {
                    NetMgr.OnMove(bc);
                }

            }

            //广播移动的坐标
            if (bc.Tp == 4)
            {
                if (NetMgr.OnMove != null)
                {
                    NetMgr.OnMove(bc);
                }
            }
        }


        //下线
        if (mid == 201)
        {
            SyncPid pid;
            pid = SyncPid.Parser.ParseFrom(bytes);
            if (NetMgr.OnOver != null)
            {
                NetMgr.OnOver(pid.Pid);
            }
            Debug.Log("Player: " + pid.Pid + " leave home");
        }

        //上线同步
        //不需要显示所有玩家  服务器只给周围玩家
        if (mid == 202)
        {
            SyncPlayers sPlayer;
            sPlayer = SyncPlayers.Parser.ParseFrom(bytes);
            Debug.Log("202 count:　"  + sPlayer.Ps.Count);
            ArrayList players = new ArrayList();
            for (int i = 0; i < sPlayer.Ps.Count; i++)
            {
                Player p = sPlayer.Ps[i];
                if (!GameMgr.PlayerIDS.Contains(p.Pid))
                {
                    //GameMgr.BornPlayer(p);
                    players.Add(p);
                    GameMgr.PlayerIDS.Add(p.Pid);
                }

            }
            GameMgr.BornPlayer(players);
        }

        if (mid == 301)
        {
            ProtoTest.FrameInfo frameInfo = ProtoTest.FrameInfo.Parser.ParseFrom(bytes);
            Loom.QueueOnMainThread(() =>
            {
                if (frameInfo.Hit != null && frameInfo.Hit.Count > 0) NetMgr.OnHit(frameInfo.Hit);
                if (frameInfo.ObjBorn!=null && frameInfo.ObjBorn.Count>0) NetMgr.OnBornBullet(frameInfo.ObjBorn);
                if (frameInfo.ObjMove != null && frameInfo.ObjMove.Count > 0) NetMgr.OnMoveBullet(frameInfo.ObjMove);
                if (frameInfo.ObjDeleted != null && frameInfo.ObjDeleted.Count > 0) NetMgr.OnBulletDeleted(frameInfo.ObjDeleted);
            });
        }
    }

    void OnReceivedMessage(byte[] bytes)
    {
        ByteBuffer buffer = new ByteBuffer(bytes);

        OnRecieveMessageDeal(buffer);
    }

    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }


    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
        loggedIn = false;
    }


    public void SendConnect(string url, int port)
    {
        ConnectServer(url, port);
        Debug.Log("Connect: " + url + ":" + port);
    }


    public void SendMessage(ByteBuffer buffer)
    {
        SessionSend(buffer.ToBytes());
        buffer.Close();
    }
}
