using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class CollisionEntrance : MonoBehaviour
{
    public int MaxObjCntPerNode;
    public int MaxDepth;
    public int XSize;
    public int YSize;
    public Vector2 BoxWidthRange;
    public Vector2 BoxHeightRange;
    public int BoxCnt;
    public bool FixedInput;
   
    private QTree qTree;
    private Player Player;

    public void Start()
    {
        Player = GeneratePlayer();
        qTree = new QTree(FixedInput ? LoadBoxes() : GenerateBoxs(), new Bound(0, 0, XSize, YSize), MaxObjCntPerNode, MaxDepth);
        //qTree.InsertObj(Player);
    }

    private List<Obstacle> GenerateBoxs()
    {
        List<Obstacle> _obsList = new List<Obstacle>();
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
            Obstacle _newObj = new Obstacle(_go, new Bound(_newX, _newY, _newWidth, _newHeight));
            _obsList.Add(_newObj);  
        }
        return _obsList;
    }

    private List<Obstacle> LoadBoxes()
    {
        Transform[] _trs = GameObject.Find("Fixed").GetComponentsInChildren<Transform>();
        List<Obstacle> _list = new List<Obstacle>();
        for(int i = 1;i<_trs.Length;i++)
            _list.Add(new Obstacle(_trs[i].gameObject));
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
        Obstacle.FixedUpdateObs(qTree);
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
            qTree.RenderTree();

            //Obstacle.OnDrawGizmosObstacle();
        }
    }
}