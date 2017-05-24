using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using ProtoTest;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    public InputField ip;
    public InputField port;

    private string mIp   = "192.168.70.6";
    private string mPort = "8999";

	// Use this for initialization
	void Start () {
	    if (ip)
	    {
            ip.text = mIp;
	    }
        if (port)
        {
            port.text = mPort;
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnLogin()
    {
        GameMgr.GameIP = ip.text;
        GameMgr.GamePort = int.Parse(port.text);
        SceneManager.LoadScene(2);
    }
}
