using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapMoveDir
{
    None,
    Down,
    Up,
}

public class TrapController : MonoBehaviour {

    [Header("插值的取值范围")]
    public float moveSpeed;
    [Header("控制陷阱是否移动")]
    public bool isMove=true;
    public Transform startTrans;
    public Transform endTrans;
    [Header("陷阱物体")]
    public Transform trapObj;

    TrapMoveDir moveDir;

	void Start () {
        if (isMove)
        {
            MovePointInit();
            moveDir = TrapMoveDir.Down;
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (isMove)
        {
            TrapMove();
        }
	}

    public void MovePointInit()
    {
        startTrans = transform.Find("TrapEndDown").transform;
        endTrans = transform.Find("TrapEndUp").transform;
        trapObj = transform.Find("TrapObj").transform;
    }

    public void TrapMove()
    {
        if (moveDir == TrapMoveDir.Down)
        {
            trapObj.transform.position += transform.up* moveSpeed * Time.deltaTime*-1;
            float distance = Vector3.Distance(trapObj.transform.position, startTrans.position);
            if (distance<0.8f)
            {
                moveDir = TrapMoveDir.Up;
            }

        }
        else if (moveDir == TrapMoveDir.Up)
        {
            trapObj.transform.position += transform.up * moveSpeed * Time.deltaTime;
            float distance = Vector3.Distance(trapObj.transform.position, endTrans.position);
            if (distance < 0.8f)
            {
                moveDir = TrapMoveDir.Down;
            }
        }
        else
        {
            Debug.Log("陷阱控制出现参数错误");
        }

    }
}
