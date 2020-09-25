using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEntrance : MonoBehaviour
{
    public int MaxObjCntPerNode;
    public int MaxDepth;
    public int XSize;
    public int YSize;
    public int BoxCnt;
    public bool FixedBox;
    public string FixedBoxRootName;

    private QTree QTree;
    public void Start()
    {
        if (!FixedBox)
        {
            QTree = new QTree(
                GenerateBoxs(),
                new Bound(new Vector2(0, 0), new Vector2(XSize, YSize)),
                MaxObjCntPerNode, MaxDepth);
        }
        else
        {
            List<Obj> _fixedBoxList = new List<Obj>();
            Transform _fixBoxRoot = GameObject.Find(FixedBoxRootName).transform;
            for (int i = 0; i < _fixBoxRoot.childCount; i++)
                _fixedBoxList.Add(new Obj(_fixBoxRoot.GetChild(i).gameObject));
            QTree = new QTree(
               _fixedBoxList,
               new Bound(new Vector2(0, 0), new Vector2(XSize, YSize)),
               MaxObjCntPerNode, MaxDepth);
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
            GameObject _go = Instantiate<GameObject>(_collidorCubePrefab, new Vector3(_newX, 0, _newY), Quaternion.identity,GameObject.Find("ColliderBoxs").transform);
            _objList.Add(new Obj(_go));
        }
        return _objList;
    }

    public void OnDrawGizmos()
    {
        if(QTree!=null)
            QTree.RenderTree();
    }
}
