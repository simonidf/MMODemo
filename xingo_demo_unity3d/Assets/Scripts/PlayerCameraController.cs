using UnityEngine;
using System.Collections;

public enum 镜头旋转类型
{
    上下旋转,
    左右旋转,
}

public class PlayerCameraController : MonoBehaviour {
    public Transform follow;
    public float 距离人物水平距离 = 5.0f;
    public float 距离人物垂直距离 = 2.0f;
    public float 距离人物横向距离 = 0f;
    public float 俯视人物角度 = 0f;
    public float 俯视人物角度可改变最大值 = 2f;
    public float CameraMoveSmooth = 0.2f;
    public float CameraAngleSmooth = 0.2f;
    private Vector3 targetPosition;

    //主要针对android平台的优化
    private int scaleWidth = 0;
    private int scaleHeight = 0;
    // Use this for initialization

    public static bool CanSmooth { get; set; }

    public static bool FreedMode { get; set; } //自由模式  假为相机跟

    public static bool DurationMode { get; set; } //当相机要回到跟随模式时使相机从一个镜头插值过度到跟随模式

    public float 镜头调整旋转角度, 镜头调整距离人物水平距离, 镜头调整距离人物垂直距离;

    void Start()
    {
        CanSmooth = false;

        follow = PlayerController.Instance.gameObject.transform;
    }

    void LateUpdate()
    {
        if (follow != null)
        {
            if (!FreedMode)
            {
                if (!CanSmooth)
                {
                    transform.rotation = Quaternion.AngleAxis(俯视人物角度, follow.right) * follow.rotation;
                    transform.position = follow.position + (-follow.forward) * 距离人物水平距离 + (follow.up) * 距离人物垂直距离 + follow.right* (距离人物横向距离);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(俯视人物角度, follow.right) * follow.rotation, CameraAngleSmooth);
                    transform.position = Vector3.Lerp(transform.position, follow.position + (-follow.forward) * 距离人物水平距离 + (follow.up) * 距离人物垂直距离 + follow.right * 距离人物横向距离, CameraMoveSmooth);
                }
            }
        }
    }

}
