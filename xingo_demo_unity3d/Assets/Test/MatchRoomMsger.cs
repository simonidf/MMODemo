using System;
using UnityEngine;
using System.Collections;

public class MatchRoomMsger : MonoBehaviour
{

    private GameWebSocket gameWS;

    private Coroutine connectAndMatchCoroutine;

    private string credential;

    public Action<string> OnMatchInfoUpdateAction;
    public Action<string> OnGameServerAddressReceiveAction;

    public void Init()
    {
        gameWS = new GameWebSocket();
    }

    public void ConnectMatchServerAndMatch(string address,string _credential)
    {
        credential = _credential;
        connectAndMatchCoroutine = StartCoroutine(ConnectAndMatch(address));
    }

    IEnumerator ConnectAndMatch(string address)
    {
        gameWS.Connect(address);
        while (!gameWS.Connected)
            yield return 0;

        gameWS.OnRceiveMsg += OnMsgReceve;
        gameWS.Request("addtoroom", credential);
    }


    public void DisConnect()
    {
        StopCoroutine(connectAndMatchCoroutine);
        gameWS.OnRceiveMsg -= OnMsgReceve;
        gameWS.Close();
    }

    private void OnMsgReceve(string msgInfo)
    {
        if (msgInfo.StartsWith("matchinfo"))
        {
            OnMatchInfoUpdate(msgInfo.Split('*')[1]);
        }
        if (msgInfo.StartsWith("gameserverinfo"))
        {
            OnGameServerInfoReceive(msgInfo.Split('*')[1]);
        }
    }

    private void OnMatchInfoUpdate(string matchInfo)
    {
        if (OnMatchInfoUpdateAction!=null) OnMatchInfoUpdateAction(matchInfo);
    }

    private void OnGameServerInfoReceive(string gameServerInfo)
    {
        if (OnGameServerAddressReceiveAction != null) OnGameServerAddressReceiveAction(gameServerInfo);
    }
}
