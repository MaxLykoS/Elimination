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
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public Bound(float x, float y, float w,float h)
    {
        X = x;
        Y = y;
        Width = w;
        Height = h;
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
                #region 该物体穿过多个子节点
                if (_intersectionTimes >= 2) 
                {
                    _Rootlist.Add(obj);
                }
                #endregion
                #region 完全在某个子节点内
                else if (_intersectionTimes==1)
                {
                    if (_biLT) _LTlist.Add(obj);
                    if (_biRT) _RTlist.Add(obj);
                    if (_biRB) _RBlist.Add(obj);
                    if (_biLB) _LBlist.Add(obj);
                }
                #endregion
                else
                    throw new Exception("正在检测父节点外的物体");
            }
        }
        int _totalCnt = _LTlist.Count() + _RTlist.Count() + _RBlist.Count() + _LBlist.Count() + _Rootlist.Count();

        if (_totalCnt <= BelongedTree.MaxObjCnt || Depth >= BelongedTree.MaxDepth)
        {
            #region 直接全部放入父节点
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
            #region 根节点物体放入根节点，子节点物体继续向下递归
            foreach (Obj obj in _Rootlist)
            {
                ObjList.Add(obj);
                obj.isLoaded = true;
                obj.BelongedNode = this;
            }
            if (_RBlist.Count() != 0)
                ChildList.Add(new QNode(_RBlist, GenRB(), depth + 1, this, BelongedTree, QTNodeType.RB));
            if (_LTlist.Count()!=0)
                ChildList.Add(new QNode(_LTlist,GenLT(),depth + 1, this, BelongedTree, QTNodeType.LT));
            if (_RTlist.Count() != 0)
                ChildList.Add(new QNode(_RTlist,GenRT(),depth + 1, this, BelongedTree, QTNodeType.RT));
            if (_LBlist.Count() != 0)
                ChildList.Add(new QNode(_LBlist,GenLB(),depth + 1, this, BelongedTree, QTNodeType.LB));
            #endregion
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

        Gizmos.DrawWireCube(new Vector3(Bound.X, 0, Bound.Y), new Vector3(Bound.Width,0, Bound.Height));
    }
    public void RenderNodeHighLight()
    {
        Gizmos.DrawWireSphere(new Vector3(Bound.X, 0, Bound.Y), Bound.Height/2);
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

    private Bound GenLT()
    {
        return new Bound(Bound.X - Bound.Width / 4, Bound.Y + Bound.Height / 4, Bound.Width / 2, Bound.Height / 2);
    }
    private Bound GenRT()
    {
        return new Bound(Bound.X + Bound.Width / 4, Bound.Y + Bound.Height / 4, Bound.Width / 2, Bound.Height / 2);
    }
    private Bound GenRB()
    {
        return new Bound(Bound.X + Bound.Width / 4, Bound.Y - Bound.Height/ 4, Bound.Width / 2, Bound.Height / 2);
    }
    private Bound GenLB()
    {
        return new Bound(Bound.X - Bound.Width / 4, Bound.Y - Bound.Height / 4, Bound.Width / 2, Bound.Height / 2);
    }
    private bool CheckIntersection(Bound b,QTNodeType type)
    {
        Bound _nb;
        switch (type)
        {
            case QTNodeType.LT: _nb = GenLT(); break;
            case QTNodeType.RT: _nb = GenRT(); break;
            case QTNodeType.RB: _nb = GenRB(); break;
            case QTNodeType.LB: _nb = GenLB(); break;
            default: throw new NotImplementedException("未指定的QTNodeType");
        }
        float[] rec1 = {b.X - b.Width / 2,b.Y - b.Height / 2,b.X + b.Width / 2,b.Y + b.Height / 2,};
        float[] rec2 = {_nb.X - _nb.Width / 2,_nb.Y - _nb.Height / 2,_nb.X + _nb.Width / 2,_nb.Y + _nb.Height / 2,};
        return !(rec1[2] <= rec2[0] || rec2[2] <= rec1[0] || rec1[3] <= rec2[1] || rec2[3] <= rec1[1]);
    }
    private bool CheckPointInside(Bound b, QTNodeType type)
    {
        switch (type)
        {
            case QTNodeType.Root:
                return (b.X <= Bound.X + Bound.Width / 2) && (b.X >= Bound.X - Bound.Width / 2) &&
                       (b.Y <= Bound.Y + Bound.Height / 2) && (b.Y >= Bound.Y - Bound.Height / 2);
            case QTNodeType.LB:
                return (b.X <= Bound.X) && (b.X >= Bound.X - Bound.Width / 2) &&
                       (b.Y <= Bound.Y) && (b.Y >= Bound.Y - Bound.Height / 2);
            case QTNodeType.LT:
                return (b.X <= Bound.X) && (b.X >= Bound.X - Bound.Width / 2) &&
                       (b.Y <= Bound.Y + Bound.Height / 2) && (b.Y >= Bound.Y);
            case QTNodeType.RB:
                return (b.X <= Bound.X + Bound.Width / 2) && (b.X >= Bound.X) &&
                       (b.Y <= Bound.Y) && (b.Y >= Bound.Y - Bound.Height / 2);
            case QTNodeType.RT:
                return (b.X <= Bound.X + Bound.Width / 2) && (b.X >= Bound.X) &&
                       (b.Y <= Bound.Y + Bound.Height / 2) && (b.Y >= Bound.Y);
            default: throw new NotImplementedException("未指定的QTNodeType");

        }
    }
}