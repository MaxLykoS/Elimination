using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Obj
{
    public float FireRate;
    public float BulletSpeed;

    private float curFireTimer;
    private GameObject bulletPrefab;

    public Player(GameObject go):base(go)
    {
        Type = ObjType.Player;
        FireRate = 0.1f;
        BulletSpeed = 1.0f;
        curFireTimer = 0;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
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
        GameObject _go = GameObject.Instantiate(bulletPrefab, Go.transform.position + direction*2, Quaternion.identity);
        Bullet _bullet = new Bullet(_go, direction, speed);
    }

    private Vector3 GetFireDirection()
    {
        Vector3 _clickPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y));
        return (_clickPos - Go.transform.position).normalized;
    }
}
