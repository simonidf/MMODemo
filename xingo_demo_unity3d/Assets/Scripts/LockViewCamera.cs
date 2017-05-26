using UnityEngine;
using System.Collections;

public class LockViewCamera : MonoBehaviour {

    public Transform follow;

    private Vector3 originalPos;

    private Vector3 followOriginalPos;

    private Vector3 originalDisVector;

    public float zoom;

    void Awake()
    {
        followOriginalPos = follow.position;
        originalPos = transform.position;

        originalDisVector = originalPos - followOriginalPos;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.mouseScrollDelta.y!=0)
        {
            if(Input.mouseScrollDelta.y>0)
            {
                zoom -= Time.deltaTime*3;
            }
            else
            {
                zoom += Time.deltaTime*3;
            }
        }

        if (zoom >= 1) zoom = 1;
        if (zoom <= 0.5f) zoom = 0.5f;

        transform.position = followOriginalPos + originalDisVector*zoom + follow.position - followOriginalPos;
	}
}
