using UnityEngine;
using System.Collections;
using System.IO;
using Pb;
using Google.Protobuf;
using System.Text;

public class PlayerController : UnitySingleton<PlayerController>
{

    public float mSpeed = 9.0f;
    public Animator playerAnimator;
    public CharacterController playerControl;
    public float 摇杆单位角度 = 22.5f;
    public float 旋转速度 = 50.0f;

    public int MID = 0;
    public bool CanMove = false;

    private bool InitComplite = false;
    private bool InitFlag = true;

    public bool CanRotate { get; set; }

    public Transform playerTransform;
    private Vector3 m_LastPosition;
    private float m_AnimatorSpeed;
    private float m_AnimatorDirection;
    private float mouseX = 0;
    private Vector3 m_CurrentMovement;
    private float m_CurrentTurnSpeed;

    private NetMgr netMgr;

    private BroadCast mBc;

    // Use this for initialization
    void Start () {

        playerTransform = gameObject.transform;

        InputGameData.Instance.RigisterEvent(OnPlayerController);

        CanRotate = true;

        netMgr = GameObject.Find("Game").GetComponent<NetMgr>();

        NetMgr.OnBorn += OnBorn;
        NetMgr.OnMove += OnMove;

        netMgr.SendConnect(GameMgr.GameIP, GameMgr.GamePort);
    }

    void OnBorn(int id)
    {
        MID = id;
    }

    void OnMove(BroadCast bc)
    {
        if (bc.Pid == MID)
        {
            mBc = bc;
            InitComplite = true;
        }
    }

    void MoveAction(float angle)
    {
        Quaternion playerRotation = Quaternion.Euler(0f, playerTransform.localEulerAngles.y, 0f);

        if (angle >= -摇杆单位角度 && angle <= 摇杆单位角度) //forward
        {
            m_CurrentMovement = playerTransform.forward * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Up;
        }
        else if (angle > 摇杆单位角度 && angle <= 3 * 摇杆单位角度) //斜右上方45度角
        {
            Vector3 v = new Vector3(1, 0, 1);
            m_CurrentMovement = playerRotation * v.normalized * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Up | (int)InputNetData.Right;
        }
        else if (angle > 3 * 摇杆单位角度 && angle <= 5 * 摇杆单位角度) //right
        {
            m_CurrentMovement = playerTransform.right * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Right;
        }
        else if (angle > 5 * 摇杆单位角度 && angle <= 7 * 摇杆单位角度) //右下
        {
            Vector3 v = new Vector3(1, 0, -1);
            m_CurrentMovement = playerRotation * v.normalized * mSpeed;

            InputGameData.NetData = (int)InputNetData.Right | (int)InputNetData.Down;
        }
        else if (angle > 7 * 摇杆单位角度 || angle <= -7 * 摇杆单位角度) //下
        {
            m_CurrentMovement = -playerTransform.forward * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Down;
        }
        else if (angle > -摇杆单位角度 * 7 && angle <= -摇杆单位角度 * 5) //左下
        {
            Vector3 v = new Vector3(-1, 0, -1);
            m_CurrentMovement = playerRotation * v.normalized * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Down | (int)InputNetData.Left;
        }
        else if (angle > -摇杆单位角度 * 5 && angle <= -摇杆单位角度 * 3) //左
        {
            m_CurrentMovement = -playerTransform.right * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Left;
        }
        else if (angle > -摇杆单位角度 * 3 && angle < -摇杆单位角度) //左上
        {
            Vector3 v = new Vector3(-1, 0, 1);
            m_CurrentMovement = playerRotation * v.normalized * mSpeed;

            InputGameData.NetData |= (int)InputNetData.Left | (int)InputNetData.Up;
        }

        playerControl.SimpleMove(mSpeed * m_CurrentMovement);


        
    }

    // Update is called once per frame
    void Update () {

        if (InitFlag && InitComplite)
        {
            InitFlag = false;
            CanMove = true;
            var tr = playerTransform;
            playerTransform.position = new UnityEngine.Vector3(mBc.P.X, mBc.P.Y, mBc.P.Z);

            //设置UI
            var name = GetComponent<PlayerInformationUIController>().textNameOwn;
            name.text = "Player_" + MID.ToString();
            Debug.Log("Start Game");
        }

	    if (playerAnimator && CanMove)
	    {
            UpdateAnimation();
            KeyBoardEvent();
	    }

        if (Input.GetMouseButtonDown(0))
        {
            SendShoot();
        }

        //Debug.Log(System.Convert.ToString(InputGameData.NetData, 2));
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

    void KeyBoardEvent()
    {

        {
            //keyborad
            if (Input.GetKey(KeyCode.W)) InputGameData.Instance.InputAngle = 1;
            if (Input.GetKeyUp(KeyCode.W))
            {
                InputGameData.Instance.InputAngle = 0;
                InputGameData.NetData &= ~(int)InputNetData.Up;
            } 
            if (Input.GetKey(KeyCode.S)) InputGameData.Instance.InputAngle = 180;
            if (Input.GetKeyUp(KeyCode.S))
            {
                InputGameData.Instance.InputAngle = 0;
                InputGameData.NetData &= ~(int)InputNetData.Down;
            }
            if (Input.GetKey(KeyCode.A)) InputGameData.Instance.InputAngle = -90;
            if (Input.GetKeyUp(KeyCode.A))
            {
                InputGameData.Instance.InputAngle = 0;
                InputGameData.NetData &= ~(int)InputNetData.Left;
            }
            if (Input.GetKey(KeyCode.D)) InputGameData.Instance.InputAngle = 90;
            if (Input.GetKeyUp(KeyCode.D))
            {
                InputGameData.Instance.InputAngle = 0;
                InputGameData.NetData &= ~(int)InputNetData.Right;
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) InputGameData.Instance.InputAngle = 30;
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) InputGameData.Instance.InputAngle = 130;
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) InputGameData.Instance.InputAngle = -150;
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) InputGameData.Instance.InputAngle = -30;


            if (Input.GetMouseButtonDown(1))
            {
                mouseX = Input.mousePosition.x;
            }
            if (Input.GetMouseButton(1) && !mouseX.Equals(Input.mousePosition.x))
            {
                if (mouseX > Input.mousePosition.x) InputGameData.Instance.RotateFalg = -1;

                if (mouseX < Input.mousePosition.x) InputGameData.Instance.RotateFalg = 1;

                mouseX = Input.mousePosition.x;

            }
            if (Input.GetMouseButtonUp(1))
            {
                InputGameData.Instance.RotateFalg = 0;
                InputGameData.NetData &= ~((int)InputNetData.Rotate_Left + (int)InputNetData.Rotate_Right);
            }
        }
    }

    void OnPlayerController(int type)
    {
        float angle = InputGameData.Instance.InputAngle;
        int rotateFlag = InputGameData.Instance.RotateFalg;
        if (type == -1)
        {
            InputGameData.Instance.GetSync = false;

        }

        //移动
        if (type == 0)
        {
            MoveAction(angle);
        }

        //旋转 调整视角的旋转
        if (CanRotate && type == 1)
        {
            if (rotateFlag == 1)
            {
                playerTransform.Rotate(new Vector3(0, 旋转速度 * Time.deltaTime, 0));

                InputGameData.NetData &= ~(int)InputNetData.Rotate_Left;
                InputGameData.NetData |= (int)InputNetData.Rotate_Right;
            }
            else
            {
                playerTransform.Rotate(new Vector3(0, -旋转速度 * Time.deltaTime, 0));

                InputGameData.NetData &= ~(int)InputNetData.Rotate_Right;
                InputGameData.NetData |= (int)InputNetData.Rotate_Left;
            }
        }

        SendPosition();
    }


    void SendPosition()
    {
        if (netMgr)
        {
            ByteBuffer b = new ByteBuffer();

            //当前位置
            Position p = new Position();
            p.X = playerTransform.position.x;
            p.Y = playerTransform.position.y;
            p.Z = playerTransform.position.z;
            p.V = playerTransform.localEulerAngles.y;

            MemoryStream ms = new MemoryStream();
            p.WriteTo(ms);
            byte[] bytes = ms.ToArray();

            b.WriteInt(bytes.Length);
            b.WriteInt(3);
            b.WriteBytes(bytes);
             
            netMgr.SendMessage(b);
        }
    }

    void SendShoot()
    {
        if (netMgr)
        {
            ByteBuffer b = new ByteBuffer();

            ProtoTest.AttackAction p = new ProtoTest.AttackAction();
            p.Pid = 1;

            MemoryStream ms = new MemoryStream();
            p.WriteTo(ms);
            byte[] bytes = ms.ToArray();

            b.WriteInt(bytes.Length);
            b.WriteInt(4);
            b.WriteBytes(bytes);

            netMgr.SendMessage(b);
        }
    }

    /*void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "test"))
        {
            SendPosition();
        }
    }*/
}
