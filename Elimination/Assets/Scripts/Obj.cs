using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj: MonoBehaviour
{
    private static LinkedList<Obj> ObjLinkedList = new LinkedList<Obj>();

    public bool HighlightNode;
    public bool HighLightObj;
    public Bound Bound;
    public bool isLoaded;
    public QNode BelongedNode;
    public bool isMoving;

    private bool isInit;
    public GameObject Go;
    private LinkedListNode<Obj> linkedListNode;

    public static void UpdateBound()
    {
        foreach (Obj obj in ObjLinkedList)
        {
            if (!obj.CheckInit())
                throw new System.Exception("Obj没有初始化！");
            obj.UpdateBoundPosition();
        }
    }

    public virtual void Init(Bound bound)
    {
        Init();
        this.Bound = bound;
    }

    public virtual void Init()
    {
        isInit = true;
        isMoving = false;
        this.Go = gameObject;
        this.Bound = new Bound(Go.transform.position.x,Go.transform.position.z,Go.transform.localScale.x,Go.transform.localScale.z);
        isLoaded = false;
        BelongedNode = null;
        HighlightNode = false;
        linkedListNode = ObjLinkedList.AddLast(this);
    }

    public void UpdateBoundPosition()
    {
        Bound.X = gameObject.transform.position.x;
        Bound.Y = gameObject.transform.position.z;
    }

    public bool CheckInit()
    {
        return isInit;
    }

    public virtual void DestroySelf()
    {
        ObjLinkedList.Remove(linkedListNode);
        Destroy(Go);
    }
}
