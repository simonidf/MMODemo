using UnityEngine;
using System.Collections;

public class HallUI : MonoBehaviour
{

    public FlowController flowController;

    public void StartMatch()
    {
        flowController.RequestMatch();
    }

    public void OnMatchStarting()
    {
        gameObject.SetActive(false);
    }
}
