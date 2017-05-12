using System;
using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public static BulletController NewBullet(ProtoTest.ObjInfo bulletInfo)
    {
        GameObject bulletPrefab = Resources.Load("Bullet") as GameObject;
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        BulletController bc = bullet.AddComponent<BulletController>();
        bc.Id = (int)bulletInfo.Id;
        bc.x = bulletInfo.X;
        bc.y = bulletInfo.Y;
        bc.z = bulletInfo.Z;
        bc.v = bulletInfo.V;

        bc.transform.position = new Vector3(bc.x,bc.y,bc.z);
        bc.transform.eulerAngles = new Vector3(0,bc.v,0);

        return bc;
    }

    public int Id;
    public float x;
    public float y;
    public float z;
    public float v;

    void Awake()
    {
        NetMgr.OnMoveBullet += OnBulletInfo;
        NetMgr.OnBulletDeleted += OnBulletInfo;
    }

    public void OnBulletInfo(Google.Protobuf.Collections.RepeatedField<ProtoTest.ObjInfo> bulletInfo)
    {
        foreach (var VARIABLE in bulletInfo)
        {
            if (VARIABLE.Id == Id)
            {
                x = VARIABLE.X;
                y = VARIABLE.Y;
                z = VARIABLE.Z;
                v = VARIABLE.V;
                transform.position = new Vector3(x, y, z);
                transform.eulerAngles = new Vector3(0, v, 0);
                if (VARIABLE.Deleted)
                {
                    OnDelete();
                }
                return;
            }
        }

    }

    public void OnDelete()
    {
        NetMgr.OnMoveBullet -= OnBulletInfo;
        Destroy(gameObject);
    }
}
