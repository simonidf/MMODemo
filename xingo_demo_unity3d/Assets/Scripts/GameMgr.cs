using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pb;
using UnityEngine.UI;


public class GameMgr : MonoBehaviour {

    public static ArrayList PlayerIDS = new ArrayList();
    public static string GameIP;
    public static int GamePort;

    private static BroadCast mBc;// 记录传过来的bc数据
    private static Player mPlayer; //记录传过来的player数据
    private static ArrayList mPlayers; //传递过来的player集合

    private static bool flag = false; //第一种实例方式
    private static bool flag1 = false; //第二种实例方式

    // Use this for initialization
    void Start () {
        mPlayers = new ArrayList();

        NetMgr.OnBornBullet += OnBornBullet;
    }
	
	// Update is called once per frame
	void Update () {
	    if (flag || flag1)
	    {
            OnBorn();
	    }
        
	}

    private void OnBornBullet(Google.Protobuf.Collections.RepeatedField<ProtoTest.ObjInfo> bulletBorn)
    {
        foreach (var VARIABLE in bulletBorn)
        {
            BulletController bc = BulletController.NewBullet(VARIABLE);
        }

    }

    private void OnBorn()
    {
        if (flag)
        {
            var player = Instantiate(Resources.Load<GameObject>("16_2"));

            var controller = player.GetComponent<AIController>();

            flag = false;
            if (controller)
            {
                controller.InitPlayer(mBc.Pid, mBc.P.X, mBc.P.Y, mBc.P.Z, mBc.P.V);
            }
            else
            {
                Debug.Log("Controller null");
            }

        }
        if (flag1)
        {
            flag1 = false;

            for (int i = 0; i < mPlayers.Count; ++i)
            {
                var player = Instantiate(Resources.Load<GameObject>("16_2"));

                var controller = player.GetComponent<AIController>();

                var data = (Player)mPlayers[i];
                controller.InitPlayer(data.Pid, data.P.X, data.P.Y, data.P.Z, data.P.V);
            }
        }

    }

    public static void BornPlayer(BroadCast bc)
    {
        mBc = bc;
        flag = true;
    }

    public static void BornPlayer(Player player)
    {
        mPlayer = player;
        flag1 = true;
    }

    public static void BornPlayer(ArrayList players)
    {
        mPlayers.Clear();
        mPlayers = players;
        flag1 = true;
    }
}
