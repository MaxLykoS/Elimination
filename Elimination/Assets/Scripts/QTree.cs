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
    public void RenderTree()
    {
        Root.RenderNode();
    }
    public void SearchNode(Obj obj)
    {
        Root.SearchNode(obj.Bound);
    }
    public void InsertObj(Obj obj)
    {
        Root.InsertObj(obj);
    }
    public void DeleteObj(Obj obj)
    {
        obj.BelongedNode.DeleteObj(obj);
    }
    public void UpdateObj(Obj obj)
    {
        // 删除后重新插入
        DeleteObj(obj);
        InsertObj(obj);
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
        List<List<Obj>> _AllLists = new List<List<Obj>> { _LTlist, _RTlist, _RBlist, _LBlist, _Rootlist };
        foreach (Obj obj in intersectionObjs)
        {
            if (obj.isLoaded == false)
            {
                #region 检测物体与几个子节点相交
                List<bool> _blist = new List<bool>
                {
                    CheckIntersection(obj.Bound, QTNodeType.LT),
                    CheckIntersection(obj.Bound, QTNodeType.RT),
                    CheckIntersection(obj.Bound, QTNodeType.RB),
                    CheckIntersection(obj.Bound, QTNodeType.LB)
                };
                int _intersectionTimes = 0;
                foreach (bool b in _blist)
                    _intersectionTimes += b ? 1 : 0;
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
                    for (int i = 0; i < 4; i++)
                        if (_blist[i])
                        {
                            _AllLists[i].Add(obj);
                            break;
                        }
                }
                #endregion
                else
                    throw new Exception("正在检测父节点外的物体");
            }
        }
        int _totalCnt = 0;
        foreach (List<Obj> list in _AllLists)
            _totalCnt += list.Count();

        if (_totalCnt <= BelongedTree.MaxObjCnt || Depth >= BelongedTree.MaxDepth)
        {
            #region 直接全部放入父节点
            foreach (List<Obj> list in _AllLists)
                foreach (Obj obj in list)
                {
                    AddObj(obj);
                }
            #endregion
        }
        else
        {
            #region 根节点物体放入根节点，子节点物体继续向下递归
            foreach (Obj obj in _Rootlist)
            {
                AddObj(obj);
            }
            #region 递归创建四个节点

            for (int i = 0; i < 4; i++)
            {
                if (_AllLists[i].Count() != 0)
                {
                    QTNodeType _type = (QTNodeType)i;
                    ChildList.Add(new QNode(_AllLists[i], GenBound(_type), depth + 1, this, BelongedTree, _type));
                }
            }
            #endregion
            #endregion
        }
    }
    public QNode(Bound bound, int depth, QNode father, QTree qTree, QTNodeType type)
    {
        this.Bound = bound;
        this.Depth = depth;
        this.Father = father;
        this.BelongedTree = qTree;
        this.Type = type;
        ObjList = new List<Obj>();
        ChildList = new List<QNode>();
    }
    public void RenderNode()
    {
            foreach (QNode node in ChildList)
            {
                node.RenderNode();
            }

        Gizmos.DrawWireCube(new Vector3(Bound.X, 0, Bound.Y), new Vector3(Bound.Width,0, Bound.Height));
        MainEntrance.RenderedNodeCnt++;
    }
    public void RenderNodeHighLight()
    {
        Gizmos.DrawWireSphere(new Vector3(Bound.X, 0, Bound.Y), Bound.Height/2);
    }
    public void SearchNode(Bound b)
    {
        if (ObjList.Count() != 0&&CheckPointInside(b,QTNodeType.Root))
        {
            // 检查每次递归遇到的节点内的物体（非叶子节点内为交叉物体，叶子节点内为非交叉物体）
            // 在这里检查碰撞
            foreach (Obj obj in ObjList)
                obj.HighLightObj = true;
        }
        // 找不到就从最可能的子节点入手继续递归的找
        foreach (QNode qNode in ChildList)
        {
            if (CheckPointInside(b, qNode.Type))
            {
                qNode.SearchNode(b);
                return;
            }
        }
    }
    public void InsertObj(Obj obj)
    {
        // 遇到最大深度的叶子，直接添加，不再递归
        if (Depth == BelongedTree.MaxDepth)
        {
            AddObj(obj);
            return;
        }
        #region 检测与节点的几个子节点相交
        List<bool> _bList = new List<bool>
        {
            CheckIntersection(obj.Bound, QTNodeType.LT),
            CheckIntersection(obj.Bound, QTNodeType.RT),
            CheckIntersection(obj.Bound, QTNodeType.RB),
            CheckIntersection(obj.Bound, QTNodeType.LB)
        };
        int _intersectionTimes = 0;
        foreach (bool b in _bList)
            _intersectionTimes += b ? 1 : 0;
        if (_intersectionTimes >= 2)  //要添加的物体与多个子节点相交
        {
            //  直接加入父节点
            AddObj(obj);
            return;
        }
        else if (_intersectionTimes == 1)  // 在子节点的位置内
        {
            QNode _node;
            for (int i = 0; i < 4; i++)
            {
                if (_bList[i])
                {
                    QTNodeType _type = (QTNodeType)i;
                    _node = GetNode((QTNodeType)i);
                    if (_node == null)
                    {
                        _node = new QNode(GenBound(_type), Depth + 1, this, BelongedTree, _type);
                        ChildList.Add(_node);
                        _node.AddObj(obj);
                    }
                    else
                    {
                        _node.InsertObj(obj);
                    }
                }
            }
        }
        else
            throw new Exception("该物体不在树的范围内");
        #endregion
    }
    public void DeleteObj(Obj obj)
    {
        ObjList.Remove(obj);
        QNode qNode = this;
        // 该节点无物体且无叶子，即删除
        while (qNode.ObjList.Count() == 0 && qNode.ChildList.Count() == 0)
        {
            QNode _deleteNode = qNode;
            qNode = qNode.Father;
            _deleteNode.Father.ChildList.Remove(_deleteNode);
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
    private Bound GenBound(QTNodeType type)
    {
        switch (type)
        {
            case QTNodeType.LT:return GenLT();
            case QTNodeType.RT:return GenRT();
            case QTNodeType.RB:return GenRB();
            case QTNodeType.LB:return GenLB();
            default:throw new Exception("不支持的type类型");
        }
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
                return (b.X < Bound.X + Bound.Width / 2) && (b.X > Bound.X - Bound.Width / 2) &&
                       (b.Y < Bound.Y + Bound.Height / 2) && (b.Y > Bound.Y - Bound.Height / 2);
            case QTNodeType.LB:
                return (b.X < Bound.X) && (b.X > Bound.X - Bound.Width / 2) &&
                       (b.Y < Bound.Y) && (b.Y > Bound.Y - Bound.Height / 2);
            case QTNodeType.LT:
                return (b.X < Bound.X) && (b.X > Bound.X - Bound.Width / 2) &&
                       (b.Y < Bound.Y + Bound.Height / 2) && (b.Y > Bound.Y);
            case QTNodeType.RB:
                return (b.X < Bound.X + Bound.Width / 2) && (b.X > Bound.X) &&
                       (b.Y < Bound.Y) && (b.Y > Bound.Y - Bound.Height / 2);
            case QTNodeType.RT:
                return (b.X < Bound.X + Bound.Width / 2) && (b.X > Bound.X) &&
                       (b.Y < Bound.Y + Bound.Height / 2) && (b.Y > Bound.Y);
            default: throw new NotImplementedException("未指定的QTNodeType");

        }
    }
    private QNode GetNode(QTNodeType type)
    {
        foreach (QNode node in ChildList)
        {
            if (node.Type == type)
            {
                return node;
            }
        }
        return null;
    }

    private void AddObj(Obj obj)
    {
        ObjList.Add(obj);
        obj.isLoaded = true;
        obj.BelongedNode = this;
    }
}