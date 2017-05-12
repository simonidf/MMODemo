using UnityEngine;
using Pb;
using System.Collections;

public class AIController : MonoBehaviour {

    NetMgr net;


    public float mSpeed = 9.0f;
    public Animator playerAnimator;
    public CharacterController playerControl;
    public float 摇杆单位角度 = 22.5f;
    public float 旋转速度 = 50.0f;
    public bool CanRotate { get; set; }
    private bool CanMove = false;
    private bool moveFlag = true;
    private bool bOver = false;
    private BroadCast mBc;
    

    private Transform playerTransform;
    private Vector3 m_LastPosition;
    private float m_AnimatorSpeed;
    private float m_AnimatorDirection;
    private int PlayerID;

    private Queue mPos = new Queue();
    private Vector3 mCurrentPos = Vector3.zero;

    private float dis = 0.3f;

    public void InitPlayer(int id, float x, float y, float z, float angle)
    {
        PlayerID = id;
        CanRotate = false;
        transform.position = new Vector3(x, y, z);
        mCurrentPos = transform.position;
        transform.rotation = Quaternion.Euler(0, angle, 0);
        playerControl.enabled = true;
    }

    // Use this for initialization
    void Start () {
        playerTransform = gameObject.transform;
        net = GameObject.Find("Game").GetComponent<NetMgr>();
        NetMgr.OnMove += OnMoveFunction;
        NetMgr.OnOver += OnOver;

        //设置UI
        var name = GetComponent<PlayerInformationUIController>().textNameOwn;
        name.text = "Player_" + PlayerID.ToString();

        //StartCoroutine("MoveLogic");
    }

    IEnumerator MoveLogic()
    {
        var pos = new Vector3(mBc.P.X, mBc.P.Y, mBc.P.Z);
        float f = 0.01f;
        while (Mathf.Abs(pos.x - playerTransform.position.x) >= f ||
            Mathf.Abs(pos.y - playerTransform.position.y) >= 0.1f ||
            Mathf.Abs(pos.z - playerTransform.position.z) >= f)
        {
            playerControl.SimpleMove(mSpeed * (pos - playerTransform.position).normalized * mSpeed);
            yield return 0;
        }
        Debug.Log("Move over");
    }

    // Update is called once per frame
    void Update () {
	    if (playerAnimator)
	    {
            UpdateAnimation();
	    }

        if (CanMove)
        {
            CanMove = false;

            var pos = new Vector3(mBc.P.X, mBc.P.Y, mBc.P.Z);
            

            //同步角度
            playerTransform.rotation = Quaternion.Euler(playerTransform.localEulerAngles.x, mBc.P.V, playerTransform.localEulerAngles.z);
            if (Vector3.Distance(playerTransform.position, pos) >= dis)
            {
                StopCoroutine("MoveLogic");
                StartCoroutine("MoveLogic");
            }
            else
            {
                playerTransform.position = new Vector3(mBc.P.X, mBc.P.Y, mBc.P.Z);
            }
        }

        if (bOver)
        {
            bOver = false;
            Destroy(gameObject);
        }
	}

    void OnDestroy()
    {
        GameMgr.PlayerIDS.Remove(PlayerID);
    }


    void UpdateAnimation()
    {
        Vector3 movementVector = playerTransform.position - m_LastPosition;

        float speed = Vector3.Dot(movementVector.normalized, playerTransform.forward);
        float direction = Vector3.Dot(movementVector.normalized, playerTransform.right);

        //blendtree set
        if (Mathf.Abs(speed) <= 0.1f && Mathf.Abs(direction) >= 0.9f) speed = 1f;
        if (speed > 0 && speed < 0.9f) speed = 1f;
        if (speed < 0 && speed > -0.9f && direction > 0.2f)
        {
            speed = -1f;
            direction = 1f;
        }
        if (speed < 0 && speed > -0.9f && direction < -0.2f)
        {
            speed = -1f;
            direction = -1f;
        }

        m_AnimatorSpeed = Mathf.MoveTowards(m_AnimatorSpeed, speed, Time.deltaTime * 5f);
        m_AnimatorDirection = Mathf.MoveTowards(m_AnimatorDirection, direction, Time.deltaTime * 5f);

        playerAnimator.SetFloat("Speed", m_AnimatorSpeed);
        playerAnimator.SetFloat("Direction", m_AnimatorDirection);

        m_LastPosition = playerTransform.position;
    }

    void OnMoveFunction(BroadCast bc)
    {
        if (PlayerID == bc.Pid)
        {
            mBc = bc;
            CanMove = true;
            CanRotate = true;
            // playerControl.Move(new Vector3(bc.P.X, transform.position.y, bc.P.Y));
        }
    }

    void OnOver(int pid)
    {
        if (pid == PlayerID)
        {
            bOver = true;
        }
    }
}
