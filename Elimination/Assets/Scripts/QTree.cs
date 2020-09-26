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

    public QNode(List<Obj> intersectionObjs, Bound bound, int depth, QNode father, QTree qTree, QTNodeType type)
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
        List<Obj> _Rootlist = new List<Obj>();
        foreach (Obj obj in intersectionObjs)
        {
            if (obj.isLoaded == false)
            {
                #region 检测物体与几个子节点相交
                bool _biLT = CheckIntersection(obj.Bound, QTNodeType.LT);
                bool _biRT = CheckIntersection(obj.Bound, QTNodeType.RT);
                bool _biRB = CheckIntersection(obj.Bound, QTNodeType.RB);
                bool _biLB = CheckIntersection(obj.Bound, QTNodeType.LB);
                int _intersectionTimes = 0;
                _intersectionTimes += _biLT ? 1 : 0; 
                _intersectionTimes += _biRT ? 1 : 0; 
                _intersectionTimes += _biRB ? 1 : 0; 
                _intersectionTimes += _biLB ? 1 : 0;
                #endregion
                if (_intersectionTimes >= 2) // 该物体穿过多个子网格
                {
                    _Rootlist.Add(obj);
                }
                else if(_intersectionTimes==1)                       //  完全在某个子节点内
                {
                    if (_biLT) _LTlist.Add(obj);
                    if (_biRT) _RTlist.Add(obj);
                    if (_biRB) _RBlist.Add(obj);
                    if (_biLB) _LBlist.Add(obj);
                }
                else
                    throw new Exception("正在检测父节点外的物体");
            }
        }
        int _totalCnt = _LTlist.Count() + _RTlist.Count() + _RBlist.Count() + _LBlist.Count() + _Rootlist.Count();

        if (_totalCnt <= BelongedTree.MaxObjCnt || Depth >= BelongedTree.MaxDepth)
        {
            #region 直接放入父节点
            foreach (Obj obj in _LTlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            foreach (Obj obj in _RTlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            foreach (Obj obj in _RBlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            foreach (Obj obj in _LBlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            foreach (Obj obj in _Rootlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            #endregion
        }
        else
        {
            foreach (Obj obj in _Rootlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            ChildList.Add(new QNode(_LTlist,GenLT(),depth + 1, this, BelongedTree, QTNodeType.LT));
            ChildList.Add(new QNode(_RTlist,GenRT(),depth + 1, this, BelongedTree, QTNodeType.RT));
            ChildList.Add(new QNode(_RBlist,GenRB(),depth + 1, this, BelongedTree, QTNodeType.RB));
            ChildList.Add(new QNode(_LBlist,GenLB(),depth + 1, this, BelongedTree, QTNodeType.LB));
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

    public void RenderNodeHighLight()
    {
        Gizmos.DrawSphere(new Vector3(Bound.Pos.x, 0, Bound.Pos.y), Bound.Rect.x /2);
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
                //if (CheckInside(b, BelongedTree.TYPES[i]))
                {

                }
            }
        }

    }

    public Bound GenLT()
    {
        return new Bound(new Vector2(Bound.Pos.x - Bound.Rect.x / 4, Bound.Pos.y + Bound.Rect.y / 4),new Vector2(Bound.Rect.x / 2, Bound.Rect.y / 2));
    }
    public Bound GenRT()
    {
        return new Bound(new Vector2(Bound.Pos.x + Bound.Rect.x / 4, Bound.Pos.y + Bound.Rect.y / 4), new Vector2(Bound.Rect.x / 2, Bound.Rect.y / 2));
    }
    public Bound GenRB()
    {
        return new Bound(new Vector2(Bound.Pos.x + Bound.Rect.x / 4, Bound.Pos.y - Bound.Rect.y / 4), new Vector2(Bound.Rect.x / 2, Bound.Rect.y / 2));
    }
    public Bound GenLB()
    {
        return new Bound(new Vector2(Bound.Pos.x - Bound.Rect.x / 4, Bound.Pos.y - Bound.Rect.y / 4), new Vector2(Bound.Rect.x / 2, Bound.Rect.y / 2));
    }

    private bool CheckIntersection(Bound b,QTNodeType type)
    {
        Bound _bound;
        switch (type)
        {
            case QTNodeType.LT: _bound = GenLT(); break;
            case QTNodeType.RT: _bound = GenRT(); break;
            case QTNodeType.RB: _bound = GenRB(); break;
            case QTNodeType.LB: _bound = GenLB(); break;
            default: throw new NotImplementedException("未指定的QTNodeType");
        }
        float[] rec1 = {
            b.Pos.x - b.Rect.x / 2,
            b.Pos.y - b.Rect.y / 2,
            b.Pos.x + b.Rect.x / 2,
            b.Pos.y + b.Rect.y / 2,
        };
        float[] rec2 = {
            _bound.Pos.x - _bound.Rect.x / 2,
            _bound.Pos.y - _bound.Rect.y / 2,
            _bound.Pos.x + _bound.Rect.x / 2,
            _bound.Pos.y + _bound.Rect.y / 2,
        };

        return !(rec1[2] <= rec2[0] || rec2[2] <= rec1[0] || rec1[3] <= rec2[1] || rec2[3] <= rec1[1]);
    }

    private bool CheckInside(Bound b, QTNodeType type)
    {
        Bound _bound;
        switch (type)
        {
            case QTNodeType.LT: _bound = GenLT(); break;
            case QTNodeType.RT: _bound = GenRT(); break;
            case QTNodeType.RB: _bound = GenRB(); break;
            case QTNodeType.LB: _bound = GenLB(); break;
            default: throw new NotImplementedException("未指定的QTNodeType");
        }
        float x_1 = b.Pos.x, x_2 = _bound.Pos.x;
        float y_1 = b.Pos.y, y_2 = _bound.Pos.y;
        float width_1 = b.Rect.x, width_2 = _bound.Rect.x;
        float height_1 = b.Rect.y, height_2 = _bound.Rect.y;

        float maxx1 = x_1 + width_1, maxx2 = x_2 + width_2;
        float minx1 = x_1 - width_1, minx2 = x_2 - width_2;
        float maxy1 = y_1 + height_1, maxy2 = y_2 + height_2;
        float miny1 = y_1 - height_1, miny2 = y_2 - height_2;
        bool totallyInside = !(maxx1 >= maxx2 && minx1 <= minx2 && maxy1 >= maxy2 && miny1 <= miny2);
        return totallyInside;
    }

    private bool CheckPointInside(Bound b, QTNodeType type)
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