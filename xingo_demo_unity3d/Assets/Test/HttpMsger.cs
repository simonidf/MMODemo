using System;
using UnityEngine;
using System.Collections;

public class HttpMsger
{
    private MonoEmpty coroutineRunner;

    public string Host;

    public void Init(string host)
    {
        GameObject go = new GameObject();
        coroutineRunner = go.AddComponent<MonoEmpty>();
        MonoBehaviour.DontDestroyOnLoad(go);
        Host = host;
    }

    public void Request(string route,WWWForm form,Action<string> cb)
    {
        coroutineRunner.StartCoroutine(SendPost(Host + "/" + route, form, cb));
    }

    IEnumerator SendPost(string _url, WWWForm _wForm,Action<string> cb)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            Debug.Log(postData.text);
            cb(postData.text);
        }
    }
}
