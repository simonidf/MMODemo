using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputNetData : int
{
    Up              = 1,
    Down            = 2,
    Left            = 4,
    Right           = 8,
    Rotate_Left     = 16,
    Rotate_Right    = 32
}



public class InputGameData : MonoBehaviour {

    private static InputGameData _instance;
    public bool CanSmooth { get; set; } //设置镜头
    public bool GetSync { get; set; } //当玩家移动停止时同步玩家停止消息

    private float angle = 0; //角度
    private int rotateFlag = 0; //0为不旋转 1为向右转 -1为向左转

    public static int NetData = 0;

    public delegate void InputDataChangedHandler(int type); 
    private event InputDataChangedHandler DataChanged;

    public void RigisterEvent(InputDataChangedHandler handler)
    {
        DataChanged += handler;
        //Debug.Log("Add");
    }

    public void RemoveEvent()
    {
        while(DataChanged != null)
        {
            this.DataChanged -= this.DataChanged;
        }
    }

    protected void OnChanged(int type)
    {
        if (DataChanged != null)
        {
            DataChanged(type);
        }
        else
        {
            Debug.Log("no listener");
        }
    }

    public static InputGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singleton = new GameObject();

                _instance = singleton.AddComponent<InputGameData>();
                singleton.name = "(singleton)" + typeof(InputGameData).ToString();
            }
            return _instance;
        }
    }

    public float InputAngle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
        }
    }

    public int RotateFalg
    {
        get
        {
            return rotateFlag;
        }
        set
        {
            rotateFlag = value;
        }
    }

    void MoveAndeRotateAction()
    {
        if (angle != 0)
        {
            OnChanged(0);
        }

        if (rotateFlag != 0)
        {
            OnChanged(1);
        }
    }

    void Update()
    {
        if (!TalkCenter.Talking)
        {
            MoveAndeRotateAction();
        } 
    }
}
