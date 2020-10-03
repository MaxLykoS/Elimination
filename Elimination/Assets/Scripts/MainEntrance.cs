using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class MainEntrance : MonoBehaviour
{
    public static int RenderedNodeCnt = 0;

    public int MaxObjCntPerNode;
    public int MaxDepth;
    public int XSize;
    public int YSize;
    public Vector2 BoxWidthRange;
    public Vector2 BoxHeightRange;
    public int BoxCnt;
    public bool FixedInput;
   
    private QTree qTree;
    private List<Obj> objList;
    private Player Player;

    public static void Destroy(GameObject go)
    {
        Destroy(go);
    }
    public void Start()
    {
        objList = FixedInput ? LoadBoxes() : GenerateBoxs();
        Player = GeneratePlayer();
        //objList.Add(Player);
        qTree = new QTree(objList, new Bound(0, 0, XSize, YSize), MaxObjCntPerNode, MaxDepth);
        #region 插入物体测试代码
        /*insertedObjs = LoadInsertedObjs();
        foreach (Obj obj in insertedObjs)
        {
            obj.Init();
            qTree.InsertObj(obj);
            objList.Add(obj);
        }*/
        #endregion
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
            Obj _newObj = new Obj(_go, new Bound(_newX, _newY, _newWidth, _newHeight));
            _objList.Add(_newObj);  
        }
        return _objList;
    }

    private List<Obj> LoadBoxes()
    {
        GameObject[] _gos = GameObject.Find("Fixed").GetComponentsInChildren<GameObject>();
        List<Obj> _list = new List<Obj>();
        foreach (GameObject _go in _gos)
            _list.Add(new Obj(_go));
        return _list;
    }

    private Player GeneratePlayer()
    {
        GameObject _collidorCubePrefab = Resources.Load<GameObject>("Prefabs/Player");
        GameObject _go = Instantiate<GameObject>(_collidorCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        _go.transform.localScale = new Vector3(1, 1, 1);
        Player _newPlayer = new Player(_go);
        return _newPlayer;
    }

    public void FixedUpdate()
    {
        Bullet.BulletsUpdate(qTree);

        foreach (Obj obj in objList)
        {
            qTree.UpdateObj(obj);
        }

        //qTree.SearchNode(Player);

        foreach (Obj obj in objList)
        {
            if (obj.HighLightObj)
            {
                obj.Go.transform.Rotate(new Vector3(1f, 1.0f, 1f));
                obj.HighLightObj = false;
            }
        }
    }

    private void Update()
    {
        Obj.UpdateBound();
        Player.PlayerUpdate();
    }
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

    /*private List<Obj> LoadInsertedObjs()
    {
        List<Obj> _insertedObjs = new List<Obj>();
        GameObject root = GameObject.Find("InsertedObjs");
        foreach (Obj obj in root.transform.GetComponentsInChildren<Obj>())
        {
            obj.Init();
            _insertedObjs.Add(obj);
        }
        return _insertedObjs;
    }*/
}