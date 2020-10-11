using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj 
{
    private static LinkedList<Obj> Objs = new LinkedList<Obj>();

    public ObjType Type;
    public GameObject Go;
    public Bound Bound;

    private LinkedListNode<Obj> objNode;

    public static void UpdateBound()
    {
        foreach (Obj obj in Objs)
        {
            obj.UpdateBoundPosition();
        }
    }

    public Obj(GameObject go)
    {
        this.Go = go;
        this.Bound = new Bound(go.transform.position.x, go.transform.position.z, go.transform.localScale.x, go.transform.localScale.z);
        this.objNode = Objs.AddLast(this);
    }

    public Obj(GameObject go, Bound b)
    {
        this.Go = go;
        this.Bound = b;
        this.objNode = Objs.AddLast(this);
    }

    public void UpdateBoundPosition()
    {
        Bound.X = Go.transform.position.x;
        Bound.Y = Go.transform.position.z;
    }

    public virtual void DestroySelf()
    {
        Objs.Remove(objNode);
        Object.Destroy(Go);
    }
}
