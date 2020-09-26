using System.Collections;
using System.Collections.Generic;
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

    private QTree QTree;
    private List<Obj> list;
    public void Start()
    {
        list = FixedInput?LoadBoxes():GenerateBoxs();

        QTree = new QTree(
            list,
            new Bound(0, 0,XSize, YSize),
            MaxObjCntPerNode, MaxDepth);
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

    public void OnDrawGizmos()
    {
        if (QTree != null)
        {
            QTree.RenderTree();
        }
        if (list != null)
        {
            foreach (Obj obj in list)
            {
                if (obj.isHighLight)
                    obj.BelongedNode.RenderNodeHighLight();
            }
        }
    }
}
