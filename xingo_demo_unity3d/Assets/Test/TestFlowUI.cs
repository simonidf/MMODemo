using UnityEngine;
using System.Collections;

public class TestFlowUI : MonoBehaviour
{

    public GameObject go;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<RectTransform>().localPosition = UnityEngine.Camera.main.WorldToScreenPoint(go.transform.position ) - new Vector3(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2, 0);
    }
}
