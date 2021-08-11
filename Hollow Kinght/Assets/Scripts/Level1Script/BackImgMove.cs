using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackImgMove : MonoBehaviour {

    public Transform player;
    Vector3 backStartPoint;     //初始背景的位置
    Vector3 playeStartPoint;    //初始玩家的位置
	void Start () {

        backStartPoint = transform.position;
        playeStartPoint = player.position;

    }
	
	// Update is called once per frame
	void Update () {
        BackMoveFunc();

    }

    public void BackMoveFunc()
    {
        float tempX = player.position.x - playeStartPoint.x;
        float tempY = player.position.y - playeStartPoint.y;    //计算X,Y两轴的历史位移
        float xVaule = tempX * 0.08f;
        float yVaule = tempY * 0.06f;//计算出背景的X，Y轴位移
        transform.position = new Vector3(backStartPoint.x + xVaule, backStartPoint.y + yVaule, 0);
    }

}
