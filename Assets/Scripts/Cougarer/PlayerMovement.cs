using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : RoleBase
{
    public int Speed = 1;

    private Vector3 targetPos;
    private Vector3 targetRotation;
    private float lerpSpeed = 1.0f;

    public PlayerMovement(int BattleID , GameObject gameObject):base(BattleID,gameObject)
    {
        targetPos = Go.transform.position;
        targetRotation = Go.transform.eulerAngles;
    }

    public override void Logic_MoveLeft()
    {
        targetPos += Vector3.left * Speed;
        targetRotation = new Vector3(0, 270, 0);

        Go.transform.eulerAngles = Vector3.up * 270.0f;
        Go.transform.Translate(Go.transform.forward * Speed, Space.World);
        //Go.transform.position += Vector3.left * Speed;
    }
    public override void Logic_MoveRight()
    {
        targetPos += Vector3.right * Speed;
        targetRotation = new Vector3(0, 90, 0);
        Go.transform.eulerAngles = Vector3.up * 90.0f;
        Go.transform.Translate(Go.transform.forward * Speed, Space.World);
        //Go.transform.position += Vector3.right * Speed;
    }
    public override void Logic_MoveUp()
    {
        targetPos += Vector3.forward * Speed;
        targetRotation = new Vector3(0, 0, 0);
        Go.transform.eulerAngles = Vector3.up;
        Go.transform.Translate(Go.transform.forward * Speed, Space.World);
        //Go.transform.position += Vector3.forward * Speed;
    }
    public override void Logic_MoveDown()
    {
        Go.transform.eulerAngles = Vector3.up * 180.0f;
        Go.transform.Translate(Go.transform.forward * Speed, Space.World);

        targetPos += Vector3.back * Speed;
        targetRotation = new Vector3(0, 180, 0);
        //Go.transform.position += Vector3.back * Speed;
    }

    public override void View_Move()
    {
        Debug.Log("目前position值为:" + Go.transform.position.x + " , " + Go.transform.position.y + " , " + Go.transform.position.z);
        Debug.Log("目标position值为:" + targetPos.x + " , " + targetPos.y + " , " + targetPos.z);

        Go.transform.position = Vector3.MoveTowards(Go.transform.position, targetPos, Time.deltaTime * lerpSpeed);
        
        Debug.Log("目前rotation值为:"+Go.transform.eulerAngles.x+ " , "+Go.transform.eulerAngles.y+" , "+ Go.transform.eulerAngles.z);
        Debug.Log("目标rotation值为:" + targetRotation.x + " , " + targetRotation.y + " , " + targetRotation.z);

        Go.transform.eulerAngles = Vector3.MoveTowards(Go.transform.rotation.eulerAngles, targetRotation, Time.deltaTime * lerpSpeed);
    }
}
