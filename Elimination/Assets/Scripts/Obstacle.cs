using System;
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

public class Obstacle : Obj
{
    private static LinkedList<Obstacle> Obss = new LinkedList<Obstacle>();
    //private static ArrayList showed = new ArrayList() {  };

    public bool HighLightObs;
    public bool isLoaded;
    public QNode BelongedNode;

    private LinkedListNode<Obstacle> obsNode;

    public static void FixedUpdateObs(QTree qTree)
    {
        /*foreach (Obstacle obs in Obss)
        {
            qTree.UpdateObj(obs);
        }*/

        foreach (Obstacle obs in Obss)
        {
            obs.HighlightSelf();
        }
    }

    public static void OnDrawGizmosObstacle()
    {
        foreach (Obstacle obs in Obss)
        {
            obs.HighlightNode();
        }
    }

    public Obstacle(GameObject go) : base(go)
    {
        Type = ObjType.Obstacle;
        isLoaded = false;
        BelongedNode = null;
        obsNode = Obss.AddLast(this);
    }

    public Obstacle(GameObject go, Bound b) : base(go, b)
    {
        Type = ObjType.Obstacle;
        isLoaded = false;
        BelongedNode = null;
        obsNode = Obss.AddLast(this);
    }

    public void HighlightSelf()
    {
        if (HighLightObs)
        {
            Go.transform.Rotate(new Vector3(1f, 1.0f, 1f));
            HighLightObs = false;
        }
    }

    public override void DestroySelf()
    {
        if (BelongedNode != null)
            BelongedNode.DeleteObj(this);
        Obss.Remove(obsNode);
        base.DestroySelf();
    }

    public void HighlightNode()
    {
        /*if (!showed.Contains(Convert.ToInt32(Go.name))) 
            return;*/
        Vector2 _nodePos = BelongedNode.GetPos();
        Vector2 _nodeSize = BelongedNode.GetSize();
        Vector3 _center = new Vector3(_nodePos.x, 0, _nodePos.y);
        Vector3 _size = new Vector3(_nodeSize.x, 1, _nodeSize.y);
        Gizmos.DrawSphere(_center, _size.x/10);
    } 
}
