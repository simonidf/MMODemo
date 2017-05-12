using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInformationUIController : MonoBehaviour {
    [HideInInspector]
    public GameObject playerUI { get; private set; }
    private GameObject playerChildUI; //UI所挂对象

    private float startDistance = 18.385f; //倍数为1时的参考距离
    private float times = 0f; //倍数

    //以下变量主要用于玩家自己的UI显示
    private GameObject playerOwnGameObject;
    public Text textNameOwn;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        playerUI = GameObject.Find("Canvas");
        playerOwnGameObject = Instantiate(Resources.Load<GameObject>("Name"));
        if (playerOwnGameObject)
        {
            textNameOwn = playerOwnGameObject.GetComponent<Text>();
            playerOwnGameObject.transform.parent = playerUI.transform;
        }

        if (gameObject.GetComponent<PlayerController>())
        {
            textNameOwn.color = Color.red;
        }
    }


    void LateUpdate()
    {
        UpdateUIPostionAndUIScale();
    }

    void UpdateUIPostionAndUIScale()
    {
        if(playerOwnGameObject)
        {
            //Vector3 v = mainCamera.WorldToScreenPoint(transform.TransformPoint(playerOwnGameObject.transform.localPosition));
            Vector3 v = mainCamera.WorldToScreenPoint(transform.position);
            //Debug.Log(v);
            if (v.z > 0)
            {
                if (Vector3.Distance(mainCamera.transform.position, transform.position) != 0)
                    times = startDistance / Vector3.Distance(mainCamera.transform.position, transform.position);
                else
                    times = 1f;

                if (
                    Vector3.Distance(mainCamera.transform.position, transform.position) <= 3)
                    times = 0f;

                if (times <= 0.1f) times = 0f;
            }
            else
                times = 0;
            playerOwnGameObject.transform.localScale = new Vector3(times * 0.5f, times * 0.5f, times * 0.5f);
            playerOwnGameObject.transform.position = new Vector3(v.x, v.y, 0);
            playerOwnGameObject.transform.position = new Vector3(v.x, v.y + 120 * times, 0);
        }
    }

    void OnDestroy()
    {
        Destroy(playerOwnGameObject);
    }
}
