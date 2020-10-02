using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Obj
{
    public float FireRate;
    public float BulletSpeed;

    private float curFireTimer;
    private GameObject bulletPrefab;
    public override void Init()
    {
        base.Init();
        FireRate = 0.1f;
        BulletSpeed = 1.0f;
        curFireTimer = 0;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Cube");
    }

    public void PlayerUpdate()
    {
        if(curFireTimer<FireRate)
            curFireTimer += Time.deltaTime;
        KbListener();
    }

    private void KbListener()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (curFireTimer >= FireRate)
            {
                curFireTimer = 0;
                SpawnBullet(GetFireDirection(), BulletSpeed);
            }
        }
    }

    private void SpawnBullet(Vector3 direction, float speed)
    {
        Bullet _b = Instantiate(bulletPrefab, transform.position + direction*2, Quaternion.identity).AddComponent<Bullet>();
        _b.Init(direction,speed);
        _b.Go.name = "Bullet";
    }

    private Vector3 GetFireDirection()
    {
        Vector3 _clickPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y));
        return (_clickPos - Go.transform.position).normalized;
    }
}
