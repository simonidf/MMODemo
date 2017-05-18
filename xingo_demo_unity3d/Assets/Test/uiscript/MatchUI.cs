using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class MatchUI : MonoBehaviour
{

    public List<GameObject> faces;

    public FlowController flowController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateFaces(string info)
    {
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(info);

        for (int i = 0; i < faces.Count; i++)
        {
            faces[i].SetActive(false);
        }

        for (int i = 0; i < jsonData.Count; i++)
        {
            faces[i].SetActive(true);
        }
    }

    public void Cancel()
    {
        flowController.EndMatch();
    }
}
