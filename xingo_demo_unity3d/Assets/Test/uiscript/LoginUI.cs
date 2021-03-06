﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{

    public FlowController flowController;

    public InputField inputId;
    public InputField inputPwd;

    public Button btnGo;

    void Start()
    {
        inputId.text = Random.Range(1000000, 99999999).ToString();
    }

    public void OnIdModify()
    {
        flowController.userid = inputId.text;
    }

    public void OnPwdModify()
    {
        flowController.password = inputPwd.text;
    }

    public void OnGo()
    {
        flowController.Run();
        gameObject.SetActive(false);
    }
}
