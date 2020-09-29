using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class MainEntrance : MonoBehaviour
{
    public int MaxObjCntPerNode;
    public int MaxDepth;
    public int XSize;
    public int YSize;
    public Vector2 BoxWidthRange;
    public Vector2 BoxHeightRange;
    public int BoxCnt;
    public bool FixedInput;
   
    private List<Obj> insertedObjs;
    private QTree qTree;
    private List<Obj> objList;
    private List<Obj> movingObj;
    private Obj Player;
    public void Start()
    {
        movingObj = new List<Obj>();
        objList = FixedInput ? LoadBoxes() : GenerateBoxs();
        Player = GeneratePlayer();
        qTree = new QTree(
            objList,
            new Bound(0, 0, XSize, YSize),
            MaxObjCntPerNode, MaxDepth);

        insertedObjs = LoadInsertedObjs();
        foreach (Obj obj in insertedObjs)
        {
            obj.Init();
            qTree.InsertObj(obj);
            objList.Add(obj);
            //qTree.DeleteObj(obj);
        }
    }

    private List<Obj> GenerateBoxs()
    {
        List<Obj> _objList = new List<Obj>();
        GameObject _collidorCubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        int _minX = 0 - XSize / 2;
        int _maxX = 0 + XSize / 2;
        int _minY = 0 - YSize / 2;
        int _maxY = 0 + YSize / 2;
        GameObject _goRoot = GameObject.Find("ColliderBoxs");
        for (int i = 0; i < BoxCnt; i++)
        {
            int _newX = Random.Range(_minX, _maxX);
            int _newY = Random.Range(_minY, _maxY);
            int _newWidth = Random.Range((int)BoxWidthRange.x, (int)BoxWidthRange.y);
            int _newHeight = Random.Range((int)BoxHeightRange.x, (int)BoxHeightRange.y);

            GameObject _go = Instantiate<GameObject>(_collidorCubePrefab, new Vector3(_newX, 0, _newY), Quaternion.identity, _goRoot.transform);
            _go.transform.localScale = new Vector3(_newWidth, 1, _newHeight);
            Obj _newObj = _go.AddComponent<Obj>();
            _newObj.Init(new Bound(_newX, _newY, _newWidth, _newHeight));
            _objList.Add(_newObj);  
        }
        return _objList;
    }

    private List<Obj> LoadBoxes()
    {
        List<Obj> _list = new List<Obj>(GameObject.Find("Fixed").transform.GetComponentsInChildren<Obj>());
        foreach (Obj obj in _list)
            obj.Init();
        return _list;
    }

    private List<Obj> LoadInsertedObjs()
    {
        List<Obj> _insertedObjs = new List<Obj>();
        GameObject root = GameObject.Find("InsertedObjs");
        foreach(Obj obj in root.transform.GetComponentsInChildren<Obj>())
        {
            obj.Init();
            _insertedObjs.Add(obj);
        }
        return _insertedObjs;
    }

    private Obj GeneratePlayer()
    {
        GameObject _collidorCubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        GameObject _go = Instantiate<GameObject>(_collidorCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        _go.transform.localScale = new Vector3(1, 1, 1);
        Obj _newObj = _go.AddComponent<Obj>();
        _newObj.Init();
        return _newObj;
    }

    public void FixedUpdate()
    {
        foreach (Obj obj in objList)
        {
            qTree.UpdateObj(obj);
        }

        qTree.SearchNode(Player);

        foreach (Obj obj in objList)
        {
            if (obj.HighLightObj)
            {
                obj.gameObject.transform.Rotate(new Vector3(1f, 1.0f, 1f));
                obj.HighLightObj = false;
            }
        }
    }

    public static int RenderedNodeCnt = 0;
    public void OnDrawGizmos()
    {
        if (qTree != null)
        {
            RenderedNodeCnt = 0;
            qTree.RenderTree();
            Debug.Log(RenderedNodeCnt);
        }
        if (objList != null&&Player!=null)
        {          
            foreach (Obj obj in objList)
            {
                if (obj.HighlightNode)
                    obj.BelongedNode.RenderNodeHighLight();
            }
        }
    }

    public void OnDestroy()
    {
        objList = null;
    }
}
