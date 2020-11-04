using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleBase : Obj
{ 
    public int BattleID;
    public RoleBase(int _bID, GameObject go) : base(go)
    {
        BattleID = _bID;
    }
    public abstract void Logic_MoveLeft();
    public abstract void Logic_MoveRight();
    public abstract void Logic_MoveUp();
    public abstract void Logic_MoveDown();
    public abstract void View_Move();
}
