using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
public enum ObjType
{
    Obstacle,
    Bullet,
    Player
}

public class Obj
{
    private static LinkedList<Obj> Objs = new LinkedList<Obj>();

    public ObjType Type;
    public bool HighLightObj;
    public Bound Bound;
    public bool isLoaded;
    public QNode BelongedNode;

    public GameObject Go;
    private LinkedListNode<Obj> objNode;

    public static void UpdateBound()
    {
        foreach (Obj obj in Objs)
        {
            obj.UpdateBoundPosition();
        }
    }

    public static void FixedUpdateObj(QTree qTree)
    {
        foreach (Obj obj in Objs)
        {
            if(obj.Type == ObjType.Obstacle)
                qTree.UpdateObj(obj);
        }

        foreach (Obj obj in Objs)
        {
            obj.HighlightSelf();
        }
    }

    public Obj(GameObject go)
    {
        Type = ObjType.Obstacle;
        this.Go = go;
        this.Bound = new Bound(Go.transform.position.x, Go.transform.position.z, Go.transform.localScale.x, Go.transform.localScale.z);
        isLoaded = false;
        BelongedNode = null;
        objNode = Objs.AddLast(this);
    }

    public Obj(GameObject go, Bound b)
    {
        Type = ObjType.Obstacle;
        this.Go = go;
        this.Bound = b;
        isLoaded = false;
        BelongedNode = null;
        objNode = Objs.AddLast(this);
    }

    public void UpdateBoundPosition()
    {
        Bound.X = Go.transform.position.x;
        Bound.Y = Go.transform.position.z;
    }

    public virtual void DestroySelf()
    {
        if(BelongedNode!=null)
            BelongedNode.DeleteObj(this);
        Objs.Remove(objNode);
        Object.Destroy(Go);
    }

    public void HighlightSelf()
    {
        if (HighLightObj)
        {
            Go.transform.Rotate(new Vector3(1f, 1.0f, 1f));
            HighLightObj = false;
        }
    }
}
