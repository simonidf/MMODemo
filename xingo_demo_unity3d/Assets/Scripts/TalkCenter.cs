using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Pb;
using System.IO;
using Google.Protobuf;

public class TalkCenter : MonoBehaviour {

    public Text mTalkText;
    public InputField mInputField;
    public static bool TalkFlag = false;
    public static string Content;
    public static string PlayerID;
    public static bool Talking = false;
    public int ContentSize = 10;

    private string mContent = "";
    private bool CanTalk = true;
    private NetMgr netMgr;

    private Queue mContents = new Queue();


    // Use this for initialization
    void Start () {
        netMgr = GameObject.Find("Game").GetComponent<NetMgr>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (mTalkText && TalkFlag)
	    {
            TalkFlag = false;
            CanTalk = true;

            mContent = "Player " + PlayerID + ": ";

            mContent += Content;

            mContent += "\n";


            

            if (mContents.Count >= ContentSize)
            {
                mContents.Dequeue();
            }
            else
            {
                mContents.Enqueue(mContent);
            }

            string str = "";
            foreach (var content in mContents)
            {
                str += content;
            }
            

            mTalkText.text = str;
        }
	}

    public void OnClickSendButton()
    {
        if (!CanTalk) return;
      
        ByteBuffer b = new ByteBuffer();

        Talk talk = new Talk();
        talk.Content = mInputField.text;

        MemoryStream ms = new MemoryStream();
        talk.WriteTo(ms);
        byte[] bytes = ms.ToArray();

        b.WriteInt(bytes.Length);
        b.WriteInt(2);
        b.WriteBytes(bytes);

        netMgr.SendMessage(b);

        CanTalk = false;
    }

    //进行输入不能进行移动等操作
    public void test1()
    {
        Talking = true;
    }

    //解除相应阻截
    public void test2()
    {
        Talking = false;
    }
}
