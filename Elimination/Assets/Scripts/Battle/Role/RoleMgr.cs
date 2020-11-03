using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class RoleMgr : MonoBehaviour
{
    public bool initFinish = false;
    private List<Transform> spawnPoints = new List<Transform>();
    private List<RoleBase> roles = new List<RoleBase>();
    private Transform playerRoot;

    void Start()
    {
        InitData();
    }

    public void InitData()
    {
        InitSpawnPoints();
        CreateRoles();

        initFinish = true;
    }

    void InitSpawnPoints()
    {
        Transform[] trs = GameObject.Find("BattleScene/SpawnPoints").GetComponentsInChildren<Transform>();
        for(int i = 1;i<trs.Length;i++)
            spawnPoints.Add(trs[i]);
    }

    void CreateRoles()
    {
        playerRoot = GameObject.Find("BattleScene/PlayersRoot").transform;
        List<BattleUserInfo> list = BattleData.Instance.list_battleUser;

        if (list.Count > spawnPoints.Count)
            Debug.LogError("出生点不足！");

        for (int i = 0; i < list.Count; i++)
        {
            CreateRole(list[i], spawnPoints[i]);
        }
    }

    void CreateRole(BattleUserInfo info, Transform transform)
    {
        GameObject _playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        roles.Add(new RoleBase(info.BattleID, Instantiate(_playerPrefab, transform.position,transform.rotation, playerRoot)));
        Debug.Log("创建角色" + info.BattleID);
    }

    public void Logic_Operation(AllPlayerOperation _op)
    {
        Debug.Log("更新操作：按键为" + _op.Operations[0].Keyinput);
        foreach (PlayerOperation op in _op.Operations)
        {
            Logic_HandleInput(roles[op.BattleID], op);
        }
    }

    void Logic_HandleInput(RoleBase role, PlayerOperation op)
    {
        switch (op.Keyinput)
        {
            case "A":role.Logic_MoveLeft();
                break;
            case "W":role.Logic_MoveUp();
                break;
            case "D":role.Logic_MoveRight();
                break;
            case "S":role.Logic_MoveDown();
                break;
            default:
                break;
        }
    }

    public void Logic_RoleUpdate()
    {
        for (int i = 1; i < roles.Count; i++)
        {
            //roles[i].PlayerUpdate();
        }
    }

    public void View_Update()
    {
        foreach (RoleBase rb in roles)
            rb.View_Move();
    }
}
