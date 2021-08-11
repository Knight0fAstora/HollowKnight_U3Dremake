using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconMove : MonoBehaviour {

    public Vector3 startPos;
    Vector3 endPos;
    Vector3 targetPos;
    float moveDistance = 58;
    RectTransform thisTrans;
    bool isMove = false;
    public float speed;
	void Start () {
		
	}
	
	void Update () {

        if (isMove)
        {
            MoveToTarget();
        }
	}
    /// <summary>
    /// 初始化图标的相应属性
    /// </summary>
    /// <param name="dir">移动方向 0-Left 1-Right </param>
    public void Init(int dir)
    {
        thisTrans = (RectTransform)transform;
        startPos = thisTrans.anchoredPosition3D; //获取面版上的UI坐标
        if (dir == 0)
        {
            endPos = startPos + Vector3.right * moveDistance;
        }
        else
        {
            endPos = startPos - Vector3.right * moveDistance;
        }
        thisTrans.anchoredPosition3D = endPos;  //移动位置
        speed = 0.3f;
    }

    public void MoveToTarget()
    {
        thisTrans.anchoredPosition3D = Vector3.Lerp(thisTrans.anchoredPosition3D, targetPos, speed);
        float distance = Vector3.Distance(thisTrans.anchoredPosition3D, targetPos);
        if (distance<2)
        {
            isMove = false;
        }
    }

    public void MouseEnterClick()
    {
        isMove = true;
        targetPos = startPos;
    }

    public void MouseExitClick()
    {
        isMove = true;
        targetPos = endPos;
    }

}
