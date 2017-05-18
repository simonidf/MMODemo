using UnityEngine;
using System.Collections;
using System;

public class WaitingHallMsger : MonoBehaviour
{

    private HttpMsger httpMsger;

    public void Init(string host)
    {
        httpMsger = new HttpMsger();
        httpMsger.Init(host);
    }

    public void Login(string userid,string password,Action<bool,string> cb)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid",userid);
        form.AddField("password",password);

        httpMsger.Request("login",form, (data) =>
        {
            LitJson.JsonData jsonObj = LitJson.JsonMapper.ToObject(data);
            string credential = jsonObj["credential"].ToString();
            Debug.Log(jsonObj["state"]);
            if (jsonObj["state"].ToString() == "200")
            {
                Debug.Log(jsonObj["credential"]);
                cb(true, credential);
            }
            else
            {
                cb(false, "");
            }
        });
    }

    public void StartMatch(string userid,string credential,Action<bool,string> cb)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("credential", credential);

        httpMsger.Request("startmatch", form, (data) =>
        {
            LitJson.JsonData jsonObj = LitJson.JsonMapper.ToObject(data);
            string matchserveraddress = jsonObj["matchserveraddress"].ToString();
            if (jsonObj["state"].ToString() == "200")
            {
                cb(true, matchserveraddress);
            }
            else
            {
                cb(false, "");
            }
        });

    }
}
