using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Obj
{
    private static LinkedList<Bullet> Bullets = new LinkedList<Bullet>();

    private float Speed;
    private Vector3 Direction;
    private LinkedListNode<Bullet> bulletNode;
    private float destroySelfOT;

    public static void BulletsUpdate(QTree qTree)
    {
        foreach (Bullet b in Bullets)
        {
            b.Move();
            b.destroySelfOT -= Time.deltaTime;
        }

        LinkedListNode<Bullet> _node = Bullets.First;
        while (_node != null)
        {
            Bullet _b = _node.Value;
            _node = _node.Next;
            if (_b.destroySelfOT <= 0)
            {
                _b.DestroySelf();
            }
            else
            {
                qTree.SearchNode(_b);
            }
        }
    }
    
    public void Init(Vector3 direction, float speed)
    {
        base.Init();
        bulletNode = Bullets.AddLast(this);
        this.Direction = direction;
        this.Speed = speed;
        destroySelfOT = 5.0f;
    }

    public override void DestroySelf()
    {
        Bullets.Remove(bulletNode);
        base.DestroySelf();
    }

    public void Move()
    {
        transform.position += Direction.normalized * Speed;
    }
}
