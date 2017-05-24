using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlowController : MonoBehaviour
{
    public string userid;

    public string password;

    public string gameCredential;

    public WaitingHallMsger waitingHallMsger;

    public MatchRoomMsger matchRoomMsger;

    public HallUI WaitingHall;
    public MatchUI MatchingUI;

    // Use this for initialization
    public void Run()
    {
        waitingHallMsger.Init("47.93.192.124:3001");
        matchRoomMsger.Init();

        waitingHallMsger.Login(userid, password, OnLoginResult);
    }

    void OnLoginResult(bool state,string credential)
    {
        if (state)
        {
            gameCredential = credential;
            WaitingHall.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("login error");
        }
    }

    [ContextMenu("RequestMatch")]
    public void RequestMatch()
    {
        waitingHallMsger.StartMatch(userid,gameCredential, OnStartMatchResult);
    }

    [ContextMenu("EndMatch")]
    public void EndMatch()
    {
        matchRoomMsger.DisConnect();
        matchRoomMsger.OnMatchInfoUpdateAction -= OnMatchInfoUpdate;
        matchRoomMsger.OnGameServerAddressReceiveAction -= OnGameServerAddressReceive;
        MatchingUI.gameObject.SetActive(false);
        WaitingHall.gameObject.SetActive(true);
    }

    void OnStartMatchResult(bool state, string matchServerAddress)
    {
        if (state)
        {
            matchRoomMsger.ConnectMatchServerAndMatch(matchServerAddress, gameCredential);
            matchRoomMsger.OnMatchInfoUpdateAction += OnMatchInfoUpdate;
            matchRoomMsger.OnGameServerAddressReceiveAction += OnGameServerAddressReceive;
            WaitingHall.OnMatchStarting();
            MatchingUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("start match failed");
        }
    }

    void OnMatchInfoUpdate(string matchInfo)
    {
        Debug.Log("current matchInfo:" + matchInfo);
        MatchingUI.UpdateFaces(matchInfo);
    }

    void OnGameServerAddressReceive(string gameServerAddress)
    {
        Debug.Log("receive gameServerAddress:" + gameServerAddress);
        StartCoroutine(GotoGame(gameServerAddress));
    }

    IEnumerator GotoGame(string gameServerAddress)
    {
        yield return  new WaitForSeconds(2);
        matchRoomMsger.DisConnect();



        GameMgr.GameIP = gameServerAddress.Split(':')[0];
        GameMgr.GamePort = int.Parse(gameServerAddress.Split(':')[1]);
        SceneManager.LoadScene(2);
    }
}
