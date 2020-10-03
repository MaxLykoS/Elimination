using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Obj
{
    private static LinkedList<Obj> ObjLinkedList = new LinkedList<Obj>();

    public bool HighlightNode;
    public bool HighLightObj;
    public Bound Bound;
    public bool isLoaded;
    public QNode BelongedNode;
    public bool isMoving;

    public GameObject Go;
    private LinkedListNode<Obj> linkedListNode;

    public static void UpdateBound()
    {
        foreach (Obj obj in ObjLinkedList)
        {
            obj.UpdateBoundPosition();
        }
    }

    public Obj(GameObject go)
    {
        isMoving = false;
        this.Go = go;
        this.Bound = new Bound(Go.transform.position.x, Go.transform.position.z, Go.transform.localScale.x, Go.transform.localScale.z);
        isLoaded = false;
        BelongedNode = null;
        HighlightNode = false;
        linkedListNode = ObjLinkedList.AddLast(this);
    }

    public Obj(GameObject go, Bound b)
    {
        isMoving = false;
        this.Go = go;
        this.Bound = b;
        isLoaded = false;
        BelongedNode = null;
        HighlightNode = false;
        linkedListNode = ObjLinkedList.AddLast(this);
    }

    public void UpdateBoundPosition()
    {
        Bound.X = Go.transform.position.x;
        Bound.Y = Go.transform.position.z;
    }

    public virtual void DestroySelf()
    {
        ObjLinkedList.Remove(linkedListNode);
        Object.Destroy(Go);
    }
}
