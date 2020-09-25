using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum QTNodeType
{
    /// <summary>
    /// 左上
    /// </summary>
    LT = 0,
    /// <summary>
    /// 右上
    /// </summary>
    RT = 1,
    /// <summary>
    /// 右下
    /// </summary>
    RB = 2,
    /// <summary>
    /// 左下
    /// </summary>
    LB = 3,
    /// <summary>
    /// 根节点
    /// </summary>
    Root = 4,
}

public class Bound
{
    public Vector2 Pos { get; set; }
    public Vector2 Rect { get; set; }

    public Bound(Vector2 pos, Vector2 rect)
    {
        this.Pos = pos;
        this.Rect = rect;
    }
}

public class QTree
{
    public List<QTNodeType> TYPES = new List<QTNodeType> { QTNodeType.LT, QTNodeType.RT, QTNodeType.RB, QTNodeType.LB };

    public int MaxObjCnt;
    public int MaxDepth;
    public QNode Root;
    public Bound Bound;
    public List<Obj> ObjList;

    public QTree(List<Obj> objList,Bound bound,int maxObjCnt,int maxDepth)
    {
        this.MaxObjCnt = maxObjCnt;
        this.ObjList = objList;
        this.MaxDepth = maxDepth;
        Bound = bound;
        Root = new QNode(objList,Bound, 0,Root, this,QTNodeType.Root);
    }

    public void InsertObj(Obj obj)
    {
        Root.InsertObj(obj);
    }

    public void RenderTree()
    {
        Root.RenderNode();
    }

    public void SearchNode(Bound b)
    {
        Root.SearchNode(b);
    }
}

public class QNode
{
    public QTNodeType Type;
    public QTree BelongedTree;
    public Bound Bound;
    public int Depth;
    public List<Obj> ObjList;
    public QNode Father;
    public List<QNode> ChildList;

    public QNode(List<Obj> insideObjs,Bound bound, int depth, QNode father, QTree qTree, QTNodeType type)
    {
        this.Bound = bound;
        this.Depth = depth;
        this.Father = father;
        this.BelongedTree = qTree;
        this.Type = type;
        ObjList = new List<Obj>();
        ChildList = new List<QNode>();

        List<Obj> _LTlist = new List<Obj>();
        List<Obj> _RTlist = new List<Obj>();
        List<Obj> _RBlist = new List<Obj>();
        List<Obj> _LBlist = new List<Obj>();
        List<List<Obj>> _lists = new List<List<Obj>>();
        _lists.Add(_LTlist); _lists.Add(_RTlist); _lists.Add(_RBlist); _lists.Add(_LBlist);
        List<QTNodeType> types = new List<QTNodeType>(4);
        foreach (Obj obj in insideObjs)
        {
            if (obj.isLoaded == false)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (CheckInside(obj.Bound,BelongedTree.TYPES[i]))
                    {
                        _lists[i].Add(obj);
                    }
                }
            }
        }
        int _totalCnt = _LTlist.Count() + _RTlist.Count() + _RBlist.Count() + _LBlist.Count();

        if (_totalCnt <= BelongedTree.MaxObjCnt|| Depth >= BelongedTree.MaxDepth)
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (Obj obj in _lists[i])
                {
                    ObjList.Add(obj);
                    obj.isLoaded = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (Obj obj in _lists[i])
                {
                    ObjList.Add(obj);
                }
            }
            Vector2 newRect = new Vector2(bound.Rect.x / 2, bound.Rect.y / 2);
            ChildList.Add(new QNode(_LTlist,
                new Bound(new Vector2(bound.Pos.x - bound.Rect.x / 4, bound.Pos.y + bound.Rect.y / 4), newRect),
                depth + 1, this, BelongedTree, QTNodeType.LT));

            ChildList.Add(new QNode(_RTlist,
                new Bound(new Vector2(bound.Pos.x + bound.Rect.x / 4, bound.Pos.y + bound.Rect.y / 4), newRect),
                depth + 1, this, BelongedTree, QTNodeType.RT));

            ChildList.Add(new QNode(_RBlist,
                new Bound(new Vector2(bound.Pos.x + bound.Rect.x / 4, bound.Pos.y - bound.Rect.y / 4), newRect),
                depth + 1, this, BelongedTree, QTNodeType.RB));

            ChildList.Add(new QNode(_LBlist,
                new Bound(new Vector2(bound.Pos.x - bound.Rect.x / 4, bound.Pos.y - bound.Rect.y / 4), newRect),
                depth + 1, this, BelongedTree, QTNodeType.LB));
        }
    }

    public void InsertObj(Obj obj)
    { 
        
    }

    public void RenderNode()
    {
        foreach (QNode node in ChildList)
        {
            node.RenderNode();
        }

        Gizmos.DrawWireCube(new Vector3(Bound.Pos.x, 0, Bound.Pos.y), new Vector3(Bound.Rect.x, 0, Bound.Rect.y));
    }

    public void SearchNode(Bound b)
    {
        if (Depth == BelongedTree.MaxDepth || ObjList.Count() <= BelongedTree.MaxObjCnt)
        {
            // 在当前节点
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (CheckInside(b, BelongedTree.TYPES[i]))
                {
                    
                }
            }
        }

    }

    private bool CheckInside(Bound b, QTNodeType type)
    {
        if (type == QTNodeType.Root)
        {
            return (b.Pos.x <= Bound.Pos.x + Bound.Rect.x / 2) && (b.Pos.x >= Bound.Pos.x - Bound.Rect.x / 2) &&
                (b.Pos.y <= Bound.Pos.y + Bound.Rect.y / 2) && (b.Pos.y >= Bound.Pos.y - Bound.Rect.y / 2);
        }
        else if (type == QTNodeType.LB)
        {
            return (b.Pos.x <= Bound.Pos.x) && (b.Pos.x >= Bound.Pos.x - Bound.Rect.x / 2) &&
                (b.Pos.y <= Bound.Pos.y) && (b.Pos.y >= Bound.Pos.y - Bound.Rect.y / 2);
        }
        else if (type == QTNodeType.LT)
        {
            return (b.Pos.x <= Bound.Pos.x) && (b.Pos.x >= Bound.Pos.x - Bound.Rect.x / 2) &&
                (b.Pos.y <= Bound.Pos.y + Bound.Rect.y / 2) && (b.Pos.y >= Bound.Pos.y);
        }
        else if (type == QTNodeType.RB)
        {
            return (b.Pos.x <= Bound.Pos.x + Bound.Rect.x / 2) && (b.Pos.x >= Bound.Pos.x) &&
                (b.Pos.y <= Bound.Pos.y) && (b.Pos.y >= Bound.Pos.y - Bound.Rect.y / 2);
        }
        else if (type == QTNodeType.RT)
        {
            return (b.Pos.x <= Bound.Pos.x + Bound.Rect.x / 2) && (b.Pos.x >= Bound.Pos.x) &&
                (b.Pos.y <= Bound.Pos.y + Bound.Rect.y / 2) && (b.Pos.y >= Bound.Pos.y);
        }
        throw new NotImplementedException("未指定的QTNodeType"); 
    }
}