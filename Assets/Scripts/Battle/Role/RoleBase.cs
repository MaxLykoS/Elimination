using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleBase : Obj
{ 
    public int BattleID;
    public int Speed = 1;

    //插值
    private Vector3 targetPos;
    private float lerpSpeed = 20.0f;
    public RoleBase(int _bID,GameObject go) : base(go)
    {
        BattleID = _bID;
        targetPos = go.transform.position;
    }

    public void Logic_MoveLeft()
    {
        targetPos += Vector3.left* Speed;
        //Go.transform.position += Vector3.left * Speed;
    }
    public void Logic_MoveRight()
    {
        targetPos += Vector3.right* Speed;
        //Go.transform.position += Vector3.right * Speed;
    }
    public void Logic_MoveUp()
    {
        targetPos += Vector3.forward* Speed;
        //Go.transform.position += Vector3.forward * Speed;
    }
    public void Logic_MoveDown()
    {
        targetPos += Vector3.back* Speed;
        //Go.transform.position += Vector3.back * Speed;
    }

    public void View_Move()
    {
        Go.transform.position = Vector3.MoveTowards(Go.transform.position, targetPos, Time.deltaTime* lerpSpeed);
    }
}
