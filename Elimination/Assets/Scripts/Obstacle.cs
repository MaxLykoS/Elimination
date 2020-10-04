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

public class Obstacle:Obj
{
    private static LinkedList<Obstacle> Obss = new LinkedList<Obstacle>();

    public bool HighLightObs;
    public bool isLoaded;
    public QNode BelongedNode;

    private LinkedListNode<Obstacle> obsNode;

    public static void FixedUpdateObs(QTree qTree)
    {
        foreach (Obstacle obs in Obss)
        {
            qTree.UpdateObj(obs);
        }

        foreach (Obstacle obs in Obss)
        {
            obs.HighlightSelf();
        }
    }

    public Obstacle(GameObject go):base(go)
    {
        Type = ObjType.Obstacle;
        isLoaded = false;
        BelongedNode = null;
        obsNode = Obss.AddLast(this);
    }

    public Obstacle(GameObject go, Bound b):base(go,b)
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
}
