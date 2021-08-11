using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 玩家现在的方向
/// </summary>
public enum PlayDir
{
    Left=1,
    Right,
}

public class MoveController : MonoBehaviour {

    public float speed;         //速度
    [Header("距离地面的最小高度")]
    public float distance;      //距离地面的高度
    public Vector3 moveSpeed;   //每一帧的移动速度
    public PlayDir nowDir;      //现在的玩家的方向
    public Animator playAnimator;

    public float gravity;       //受到的重力
    public bool gravityEnable;  //重力开关
    public bool inputEnable;    //接受输入开关  true 游戏接受按键输入  false不接受按键输入
    public float jumpPower;     //向上跳跃的力
    public bool isGround;       //是否在地面  true在地面 false不在地面
    public float jumpTime;      //跳跃的蓄力时间
    public int jumpCount;
    [Header("玩家是否存活")]
    public bool isAlive;
    [Header("攻击后坐力")]
    public float recoilForce;
    [Header("爬强状态")]
    public bool isClimb;
    [Header("跳跃状态")]
    public bool jumpState;
    [Header("超级冲刺准备时间")]
    public float sSprintTime;
    [Header("暗影冲刺持续时间")]
    public float sprintTime;
    [Header("是否进入超级冲刺状态")]
    public bool isSSState;
    [Header("是否能够使用冲刺技能")]
    public bool isCanSprint;
    [Header("是否能够使用暗影冲刺")]
    public bool isShadowSprint;
    [Header("超级冲刺特效动画机")]
    public Animator effectAnimtor;
    float timeJump;             //跳跃当前的蓄力时间
    float sSTime;               //超级冲刺当前的蓄力时间
    public Vector2 boxSize;
    int playerLayerMask;

    GameObject knifeEffectOne;  
    GameObject knifeEffectTwo;  
    GameObject knifeEffectUp;  
    GameObject knifeEffectDown;  //刀光特效物体
    public Image dieMaskImage;   //死亡遮罩
    public Vector3 startPoint;   //复活点
    GameObject jumpTwoEffect;   //二段跳特效物体
    GameObject particleObj;     //超级冲刺物体(紫色水晶)
    List<GameObject> particleObjs;
    public GameObject effectComplate;   //超级冲刺准备完成父物体
    List<GameObject> effectObjs;        //用于形成特效的物体(闪光特效)
    public GameObject superTrailing;    //超级冲刺的拖尾物体

    public GameObject shadowRePlay;     //暗影冲刺的回复特效物体
    public GameObject shadowEffect;     //暗影冲刺拖尾特效物体
    public GameObject shadowHalo;       //暗影冲刺身上的光环物体
    public GameObject shadowPartic;     //暗影冲刺的粒子物体
    [Header("暗影冲刺的Cd时间")]
    public float shadowCd;              //暗影冲刺Cd时间
    float nowShadowTime;                //现在的暗影冲刺的计时

    Quaternion leftQua;                 //超级冲刺特效地面水晶在墙上的旋转
    Quaternion rightQua;

    GameObject attactEffectObj;     //攻击交互预制体(特效)

    void Start()
    {

        isAlive = true;
        nowDir = PlayDir.Left;
        boxSize = new Vector2(0.66f, 1.32f);    //设置盒子射线的大小
        startPoint = transform.position;
        //startPoint.y = -2.56f;      //初始高度修正
        jumpTwoEffect = Resources.Load<GameObject>("Prefebs/plumeEffectOne");
        attactEffectObj = Resources.Load<GameObject>("Prefebs/AttackEffectOne");
        playAnimator = GetComponent<Animator>();
        knifeEffectOne = transform.Find("LRAttackImage").gameObject;
        knifeEffectTwo = transform.Find("LRAttackImage2").gameObject;
        knifeEffectUp = transform.Find("UDAttackImage (1)").gameObject;
        knifeEffectDown = transform.Find("UDAttackImage").gameObject;
        knifeEffectOne.SetActive(false);
        knifeEffectTwo.SetActive(false);
        knifeEffectUp.SetActive(false);
        knifeEffectDown.SetActive(false);   //初始化刀光物体，并关闭

        gravityEnable = true;
        inputEnable = true;
        jumpState = false;
        isSSState = false;
        isShadowSprint = true;
        isCanSprint = true;   //状态初始化

        playerLayerMask = LayerMask.GetMask("Player");
        playerLayerMask = ~playerLayerMask;             //获得当前玩家层级的mask值，并使用~运算，让射线忽略玩家层检测

        InitSupreObjs();
        HideSuperObjs();
        levelTime = sSprintTime / 6;    //初始化超级冲刺等级的时间


        particleObj.transform.Rotate(new Vector3(0, 0, -90));//初始化左右旋转后的四元素，用于后面特效转向
        leftQua = particleObj.transform.rotation;
        particleObj.transform.Rotate(new Vector3(0, 0, 180));
        rightQua = particleObj.transform.rotation;
        particleObj.transform.Rotate(new Vector3(0, 0, -90));
        effectAnimtor = transform.Find("SuperSprint/GatherEffect").GetComponent<Animator>();
        effectAnimtor.gameObject.SetActive(false);


        shadowHalo = Resources.Load<GameObject>("Prefebs/shadowHalo");
        shadowPartic = Resources.Load<GameObject>("Prefebs/ShadowPartic");
        shadowEffect = transform.Find("shadowTrailing").gameObject;
        shadowRePlay = transform.Find("shadowReply").gameObject;
        shadowEffect.SetActive(false);
        shadowRePlay.SetActive(false);

        InitRunAudioObj();
    }

    void Update() {
        if (!isAlive)
        {
            return; //  死亡不进行任何操作
        }
        LRMove();
        UpdateAnimtorState();
        UDMpve();
        Jump();
        AttackFunc();
        SprintFunc();
        SuperSprintFunc();
        if (isSSState)
        {
            if (Input.GetKeyDown(InputManager.Instance.jumpKey))
            {
                isSSState = false;
                StartCoroutine(SprintSlowdown(true));
                AudioManger.Instance.DeleteClipObj(1);
            }
        }
        playAnimator.SetBool("IsGround",isGround);
        CheckNextMove();
    }

    /// <summary>
    ///根据落地状态更新动画以及玩家的状态信息
    /// </summary>
    public void UpdateAnimtorState()
    {
        if (isGround)
        {
            playAnimator.SetBool("IsJump", false);
            playAnimator.ResetTrigger("IsJumpTwo");//重置2段跳触发器
            jumpCount = 0;
            playAnimator.SetBool("IsDown", false);
            jumpState = false;
            if (isClimb)
            {
                isClimb = false;
            }
            isCanSprint = true;
        }
        else
        {
            if (!jumpState)
            {
                playAnimator.SetBool("IsDown", true);
            }
            else
            {
                playAnimator.SetBool("IsDown", false);
            }
            if (isClimb)
            {
                jumpCount = 0;  //跳跃次数重置
            }
        }
    }

    /// <summary>
    /// 左右移动
    /// </summary>
    public void LRMove()
    {
        if (!inputEnable)
        {
            return;
        }
        float h = 0;
        if (Input.GetKey(InputManager.Instance.moveLeftKey))
        {
            h = -1;
        }
        else if (Input.GetKey(InputManager.Instance.moveRightKey))
        {
            h = 1;
        }
        else
        {
            h = 0;
        }

        moveSpeed.x = h * speed;

        if (!isClimb)   //爬墙状态不能通过按键转向
        {
            DirToRotate();
        }

        if (h == 0)//停止按键输入
        {
            playAnimator.SetTrigger("stopTrigger");
            playAnimator.ResetTrigger("IsRotate");
            playAnimator.SetBool("IsRun", false);
        }
        else
        {
            playAnimator.ResetTrigger("stopTrigger");
        }

        PlayRunAudio(h);
    }

    /// <summary>
    /// 根据方向进行旋转
    /// </summary>
    public void DirToRotate()
    {
        if (nowDir == PlayDir.Left && moveSpeed.x > 0)
        {
            transform.Rotate(0, 180, 0);
            nowDir = PlayDir.Right;
            if (isGround)
            {
                playAnimator.SetTrigger("IsRotate");
            }

        }
        else if (nowDir == PlayDir.Right && moveSpeed.x < 0)
        {
            transform.Rotate(0, -180, 0);
            nowDir = PlayDir.Left;
            if (isGround)//在地面才播放转向动画
            {
                playAnimator.SetTrigger("IsRotate");
            }
        }
        else if (nowDir == PlayDir.Right && moveSpeed.x > 0)
        {
            playAnimator.SetBool("IsRun", true);
        }
        else if (nowDir == PlayDir.Left && moveSpeed.x < 0)
        {
            playAnimator.SetBool("IsRun", true);
        }

    }

    /// <summary>
    /// 重力更新
    /// </summary>
    public void UDMpve()
    {
        if (!gravityEnable)
        {
           // moveSpeed.y = 0;
            return;
        }

        if (isGround)   //在地面
        {
            moveSpeed.y = 0;
        }
        else
        {
            if (isClimb)
            {
                moveSpeed.y = -1.0f;
            }
            else
            {
                moveSpeed.y += -1 * gravity * Time.deltaTime;
            }

        }
    }

    /// <summary>
    /// 检测是否在地面
    /// </summary>
    /// <returns></returns>
    public bool CheckIsGround()
    {
        float aryDistance = boxSize.y * 0.5f + 0.1f;
        RaycastHit2D hit2D = Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.down,5f, playerLayerMask);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * aryDistance, Color.red, 6.0f);
        if (hit2D.collider != null)
        {

            float tempDistance = Vector3.Distance(transform.position, hit2D.point);
            if (tempDistance > (boxSize.y * 0.5f + distance))
            {
                //transform.position += new Vector3(0, moveDistance.y, 0);
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    public void Jump()
    {
        if (!inputEnable)
        {
            return;
        }
        if (isClimb && Input.GetKeyDown(InputManager.Instance.jumpKey))
        {
            AudioManger.Instance.PlayAudio("hero_wall_jump", transform.position);
            StartCoroutine(ClimpJumpMove());
            return;
        }


        if (Input.GetKeyDown(InputManager.Instance.jumpKey))
        {
            jumpState = true;
            if (!isGround && jumpCount < 2)
            {
                jumpCount = 2;
            }
            else
            {
                jumpCount++;
            }

            if (jumpCount == 1)
            {
                moveSpeed.y += jumpPower;
                playAnimator.SetBool("IsJump", true);   //播放一段跳动画
                AudioManger.Instance.PlayAudio("hero_jump", transform.position);
            }
            else if (jumpCount == 2) 
            {
                playAnimator.SetTrigger("IsJumpTwo");    //播放二段跳动画
                moveSpeed.y = jumpPower;
                var effectObj = Instantiate(jumpTwoEffect, transform.position,Quaternion.Euler(180,0,0));    //生成粒子特效物体
                effectObj.transform.position += new Vector3(0, 0.5f, 0);    //为了效果，调高生成位置
                AudioManger.Instance.PlayAudio("jump_out_of_snow", transform.position);
            }
            timeJump = 0;
        }
        else if (Input.GetKey(InputManager.Instance.jumpKey) && jumpCount<=2 && jumpState)
        {
            timeJump += Time.deltaTime;
            if (timeJump < jumpTime)
            {
                moveSpeed.y += jumpPower;
            }
        }
        else if (Input.GetKeyUp(InputManager.Instance.jumpKey))
        {
            jumpState = false;
             timeJump = 0;
        }

        //进入上跳减速状态，但还在上升
        if (moveSpeed.y > 0 && moveSpeed.y < 1.5f)
        {
            playAnimator.SetBool("IsSlowUp", true);   
        }
        else
        {
            playAnimator.SetBool("IsSlowUp", false);   
        }

        //进入下落状态
        if (moveSpeed.y < 0)
        {

            playAnimator.SetBool("IsStopUp", true);   
        }
        else
        {
            playAnimator.SetBool("IsStopUp", false);   

        }
    }

    /// <summary>
    /// 攻击函数
    /// </summary>
    public void AttackFunc()
    {
        if (!inputEnable || isClimb)    //爬墙状态与无法按键获取时  无法攻击
        {
            return;
        }
        if (Input.GetKeyDown(InputManager.Instance.attackKey))
        {
            if (Input.GetKey(InputManager.Instance.moveUpKey))    //  向上攻击
            {
                playAnimator.SetTrigger("IsAttackUp");
                StartCoroutine(LookKnifeObj(knifeEffectUp,3));
                CheckAckInteractive(3);
            }
            else if (Input.GetKey(InputManager.Instance.moveDownKey) && !isGround) //  向下攻击且不在地面
            {
                playAnimator.SetTrigger("IsAttackDown");
                StartCoroutine(LookKnifeObj(knifeEffectDown, 3));
                CheckAckInteractive(4);

            }
            else    //左右攻击
            {
                playAnimator.SetTrigger("IsAttackLR1");
                StartCoroutine(LookKnifeObj(knifeEffectOne, 4));
                CheckAckInteractive((int)nowDir);

            }
        }
    }

    /// <summary>
    /// 检测是否攻击到了可交互物体 1,2左右 3上 4下
    /// </summary>
    /// <returns></returns>
    public void CheckAckInteractive(int dir) 
    {
        float distance = 1.8f;          //射线的检测长度
        RaycastHit2D hit2D = new RaycastHit2D();
        Vector2 raySize = new Vector2(boxSize.x + 0.5f, boxSize.y);         //扩大检测X轴范围
        switch (dir)
        {
            case 1:
                hit2D = Physics2D.BoxCast(transform.position, raySize, 0, Vector2.left, distance, playerLayerMask);
                break;
            case 2:
                hit2D = Physics2D.BoxCast(transform.position, raySize, 0, Vector2.right, distance, playerLayerMask);
                break;
            case 3:
                hit2D = Physics2D.BoxCast(transform.position, raySize, 0, Vector2.up, distance, playerLayerMask);
                break;
            case 4:
                hit2D = Physics2D.BoxCast(transform.position, raySize, 0, Vector2.down, distance, playerLayerMask);
                break;
        }

        if (hit2D.collider != null)
        {
            if (hit2D.collider.gameObject.CompareTag("Trap"))   //如果是陷阱就有后坐力
            {

                StartCoroutine(InteractiveMove(dir, 10));
                AudioManger.Instance.PlayAudio("sword_hit_reject", transform.position);
                var tempObj = Instantiate(attactEffectObj, hit2D.point, Quaternion.identity);//攻击交互特效
                switch (dir)
                {
                    case 1:
                        tempObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                        break;
                    case 2:
                        tempObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                        break;
                    case 3:
                        tempObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 4:
                        AttackRestState();
                        break;
                }
            }
            else
            {
                if (dir == 4)
                {
                    AudioManger.Instance.PlayAudio("sword_2", transform.position);
                }
                else
                {
                    AudioManger.Instance.PlayAudio("sword_1", transform.position);
                }
            }
        }
        else
        {
            if (dir == 4)
            {
                AudioManger.Instance.PlayAudio("sword_2", transform.position);
            }
            else
            {
                AudioManger.Instance.PlayAudio("sword_1", transform.position);
            }
        }
    }

    /// <summary>
    /// 攻击重置动作的相关状态
    /// </summary>
    public void AttackRestState()
    {
        jumpCount = 0;
        isCanSprint = true; //重置重置状态
    }

    /// <summary>
    /// 冲刺函数
    /// </summary>
    public void SprintFunc()
    {
        if (!inputEnable)
        {
            return;
        }
        if (Input.GetKeyDown(InputManager.Instance.sprintKey) && isCanSprint)
        {
            if (isClimb)
            {
                ClimpRotate();  //如果是爬墙状态冲刺，先转向在进行冲刺
            }
            StartCoroutine(SprintMove(sprintTime));
            if (isShadowSprint)
            {
                if (isGround)
                {
                    playAnimator.SetTrigger("IsGroundSprint");//播放冲刺动画
                }
                else
                {
                    playAnimator.SetTrigger("IsFlySprint");//播放冲刺动画
                }
                isShadowSprint = false;
                StartCoroutine(PlayShadowEffect());
                AudioManger.Instance.PlayAudio("hero_shade_dash_2", transform.position);
            }
            else
            {
                playAnimator.SetTrigger("IsSprint");//播放冲刺动画
                AudioManger.Instance.PlayAudio("hero_wings",transform.position);

            }


            isCanSprint = false;
        }
    }

    /// <summary>
    /// 播放暗影冲刺特效
    /// </summary>
    IEnumerator PlayShadowEffect()
    {
        shadowEffect.SetActive(true);

        GameObject tempObjOne = Instantiate(shadowHalo, transform.position, Quaternion.identity);
        GameObject tempObjTwo = Instantiate(shadowPartic, transform);
        tempObjTwo.transform.position += new Vector3(0, 0.9f, 0);
        yield return new WaitForSeconds(0.1f);
        shadowEffect.SetActive(false);
        yield return new WaitForSeconds(sprintTime-0.1f);
        tempObjTwo.transform.SetParent(null);
        shadowRePlay.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        AudioManger.Instance.PlayAudio("hero_shade_dash_charge_pt_2", transform.position);
        yield return new WaitForSeconds(0.3f);
        shadowRePlay.SetActive(false);
        isShadowSprint = true;
    }


    /// <summary>
    /// 刷新暗影冲刺的时间
    /// </summary>
    public void UpdateShadowCd()
    {
        if (!isShadowSprint)
        {
            nowShadowTime += Time.deltaTime;
            if (nowShadowTime>shadowCd)
            {
                isShadowSprint = true;
                nowShadowTime = 0;
            }
        }
    }

    /// <summary>
    /// 超级冲刺
    /// </summary>
    public void SuperSprintFunc()
    {

        if (!isGround && !isClimb)
        {
            return;
        }


        if (Input.GetKeyDown(InputManager.Instance.superKey))
        {
            playAnimator.ResetTrigger("IsSuperStop");//重置停止触发器
            inputEnable = false;
            moveSpeed.x = 0;
            sSTime += Time.deltaTime;
            playAnimator.SetTrigger("IsSuperReady");    //播放准备动作
            if (isClimb)
            {
                gravityEnable = false;
                moveSpeed.y = 0;
                if (nowDir == PlayDir.Right)//超级冲刺特效物体转向
                {
                    particleObj.transform.rotation = rightQua;
                    particleObj.transform.position += Vector3.right * 0.2f;
                }
                else
                {
                    particleObj.transform.rotation = leftQua;
                    particleObj.transform.position += Vector3.left * 0.2f;
                }
                particleObj.transform.position += Vector3.up * 0.41f;
            }
            lookLevel = 0;          //重置特效等级
            effectAnimtor.gameObject.SetActive(true);
            effectAnimtor.SetTrigger("isPlayEffect");
            AudioManger.Instance.PlayAudio("hero_super_dash_charge", transform.position);
        }
        else if (Input.GetKey(InputManager.Instance.superKey))
        {
            sSTime += Time.deltaTime;
            LookParticObj(sSTime);
            if ((sSTime > sSprintTime) && !isPlayState)
            {
                isPlayState = true;
                AudioManger.Instance.PlayAudio("hero_super_dash_ready", transform.position);
                StartCoroutine(PlayEffectSuper());
            }
        }
        else if (Input.GetKeyUp(InputManager.Instance.superKey))
        {
            if (sSTime > sSprintTime)       //蓄力时间满足条件
            {

                gravityEnable = false;
                moveSpeed.y = 0;
                if (isClimb)
                {

                    ClimpRotate();  //如果时爬墙状态，先转向在设定移动方向
                    isClimb = false;
                    playAnimator.ResetTrigger("IsClimb"); //重置触发器  退出
                    playAnimator.SetTrigger("exitClimp");
                }
                if (nowDir == PlayDir.Left)
                {
                    moveSpeed.x = 20 * -1;
                }
                else
                {
                    moveSpeed.x = 20;
                }
                isSSState = true;
                playAnimator.SetTrigger("IsSuperStart");//播放冲刺动作

                superTrailing.SetActive(true);     //打开拖尾物体的显示
                AudioManger.Instance.PlayAudio("hero_super_dash_loop", transform,1);
                Debug.Log("超级冲刺开始");
            }
            else  //不满足
            {
                if (isClimb)
                {
                    gravityEnable = true;

                }
                playAnimator.SetTrigger("IsSuperStop");//停止冲刺
                inputEnable = true;
            }
            particleObj.transform.localRotation = Quaternion.identity;
            particleObj.transform.localPosition = new Vector3(0,-0.61f,0);  //设定好的高度
            HideSuperObjs();
            sSTime = 0;
            effectAnimtor.SetTrigger("isStopEffect");
            effectAnimtor.gameObject.SetActive(false);
            isPlayState = false;
        }

    }

    /// <summary>
    /// 隐藏特效物体的显示
    /// </summary>
    public void HideSuperObjs()
    {
        for (int i=0;i<particleObjs.Count;i++)
        {
            particleObjs[i].SetActive(false);
        }
    }

    int lookLevel = 0;  //特效物体的显示等级
    float levelTime = 0;

    /// <summary>
    /// 查看超级冲刺特效物体
    /// </summary>
    public void LookParticObj(float time)
    {
        if (lookLevel>=6)
        {
            return;
        }
        if (time> levelTime*lookLevel)  //如果现在的时间大于计算出来的时间
        {
            GameObject tempObj = particleObjs[lookLevel];
            SpriteRenderer[] renderers = tempObj.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i=0;i<renderers.Length;i++)
            {
                bool isLookPartic = ParticObjIsLook(renderers[i].transform);
            }
            tempObj.SetActive(true);
            lookLevel++;
        }
    }

    /// <summary>
    /// 检测特效物体是否能够显示
    /// </summary>
    /// <returns></returns>
    public bool ParticObjIsLook(Transform objTrs)
    {
        RaycastHit2D hit2D;
        if (isClimb)        //如果进行了爬墙状态
        {
            if (nowDir == PlayDir.Right)
            {
                hit2D = Physics2D.Raycast(objTrs.position, Vector3.right, boxSize.y, playerLayerMask);

            }
            else
            {
                hit2D = Physics2D.Raycast(objTrs.position, Vector3.left, boxSize.y, playerLayerMask);

            }
        }
        else
        {
            hit2D = Physics2D.Raycast(objTrs.position, Vector3.down, boxSize.y, playerLayerMask);
        }

        if (hit2D.transform!=null)
        {
            Debug.Log("显示特效物体");
            objTrs.gameObject.SetActive(true);
            return true;
        }
        objTrs.gameObject.SetActive(false);
        return false;
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void Die()
    {
        //碰到陷阱就死亡  就是少血
        isAlive = false;
        StartCoroutine(DieAnimator());
    }

    /// <summary>
    /// 死亡动画
    /// </summary>
    /// <returns></returns>
    IEnumerator DieAnimator()
    {
        dieMaskImage.gameObject.SetActive(true);
        dieMaskImage.color = new Color32(0, 0, 0, 0);
        Color32 lookColor = new Color32(0, 0, 0, 255);
        Color32 hideColor = new Color32(0, 0, 0, 0);

        for (int i=1;i<=10;i++)
        {
            Color32 targetColor = Color32.Lerp(hideColor, lookColor, i * 0.1f);
            dieMaskImage.color = targetColor;

            yield return null;
        }
        transform.position = startPoint;

        isAlive = true;
        for (int i = 1; i <=10 ; i++)
        {
            Color32 targetColor = Color32.Lerp(lookColor, hideColor, i * 0.1f);
            dieMaskImage.color = targetColor;
            yield return new WaitForSeconds(0.05f);
        }
        dieMaskImage.gameObject.SetActive(false);

    }

    IEnumerator SprintMove(float time)
    {

        inputEnable = false;
        gravityEnable = false;
        moveSpeed.y = 0;


        if (nowDir == PlayDir.Left)
        {
            moveSpeed.x = 15*-1;
        }
        else
        {
            moveSpeed.x = 15;
        }
        yield return new WaitForSeconds(time);
        inputEnable = true;
        gravityEnable = true;
    }

    /// <summary>
    /// 超级冲刺减速
    /// </summary>
    IEnumerator SprintSlowdown(bool isMove)
    {
        superTrailing.SetActive(false);             //关闭拖尾显示
        playAnimator.SetTrigger("IsSuperStop");
        if (isMove)
        {
            float speed;
            speed = moveSpeed.x;

            for (int i = 1; i <= 10; i++)
            {
                if (i % 2 == 0)
                {
                    speed = speed / 2;
                }
                yield return null;
            }
        }

        inputEnable = true;
        gravityEnable = true;
    }

    /// <summary>
    /// 墙上跳跃的移动
    /// </summary>
    /// <returns></returns>
    IEnumerator ClimpJumpMove()
    {
        inputEnable = false;    //此时不接受其余输入
        gravityEnable = false;
        isClimb = false;
        playAnimator.SetTrigger("IsStopClimpJump");

        playAnimator.ResetTrigger("IsClimb");//重重爬墙触发器
        if (nowDir == PlayDir.Left)
        {
            moveSpeed.x = 8;
        }
        else
        {
            moveSpeed.x = -8;
        }

        moveSpeed.y =  6;
        yield return new WaitForSeconds(0.15f);
        inputEnable = true;
        gravityEnable = true;

    }

    /// <summary>
    /// 显示隐藏的刀光特效物体
    /// </summary>
    /// <param name="knifeObj">刀光物体</param>
    /// <param name="frameCount">持续帧数</param>
    /// <returns></returns>
    IEnumerator LookKnifeObj(GameObject knifeObj,int frameCount)
    {
        knifeObj.SetActive(true);
        for (int i=0;i<frameCount;i++)
        {
            yield return null;
        }
        knifeObj.SetActive(false);
    }

    /// <summary>
    /// 攻击交互物体后的移动
    /// </summary>
    /// <param name="dir">方向</param>
    /// <returns></returns>
    IEnumerator InteractiveMove(int dir,int frameCount)
    {
        inputEnable = false;
        for (int i=0;i<frameCount;i++)
        {
            switch (dir)
            {
                case 1:
                    moveSpeed.x = recoilForce;
                    break;
                case 2:
                    moveSpeed.x = -recoilForce;
                    break;
                case 3:
                    moveSpeed.y = -recoilForce;
                    break;
                case 4:
                    moveSpeed.y = recoilForce;
                    break;
            }

            yield return null;
        }
        inputEnable = true;
        yield return null;
    }

    /// <summary>
    /// 爬墙跳后的转向
    /// </summary>
    public void ClimpRotate()
    {
        if (nowDir == PlayDir.Left)
        {
            nowDir = PlayDir.Right;
            transform.Rotate(0, 180, 0);
        }
        else
        {
            nowDir = PlayDir.Left;
            transform.Rotate(0, -180, 0);
        }
    }

    /// <summary>
    /// 检测下一帧的位置是否能够移动，并进行修正
    /// </summary>
    public void CheckNextMove()
    {
        Vector3 moveDistance = moveSpeed * Time.deltaTime;
        int dir = 0;//确定下一帧移动的左右方向
        if (moveSpeed.x > 0)
        {
            dir = 1;
        }
        else if (moveSpeed.x < 0)
        {
            dir = -1;
        }
        else
        {
            dir = 0;//左右方向没有移动
        }
        if (dir != 0)//当左右速度有值时
        {
            RaycastHit2D lRHit2D = Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.right * dir, 5.0f, playerLayerMask);
            if (lRHit2D.collider != null)//如果当前方向上有碰撞体
            {
                float tempXVaule = (float)Math.Round(lRHit2D.point.x, 1);                   //取X轴方向的数值，并保留1位小数精度。防止由于精度产生鬼畜行为
                Vector3 colliderPoint = new Vector3(tempXVaule, transform.position.y);      //重新构建射线的碰撞点
                float tempDistance = Vector3.Distance(colliderPoint, transform.position);   //计算玩家与碰撞点的位置
                DrawBoxLine(lRHit2D.point, boxSize, 3.0f);
                if (tempDistance > (boxSize.x * 0.5f + distance))   //如果距离大于 碰撞盒子的高度的一半+最小地面距离
                {
                    transform.position += new Vector3(moveDistance.x, 0, 0); //说明此时还能进行正常移动，不需要进行修正
                    if (isClimb)        //如果左右方向没有碰撞体了，退出爬墙状态
                    {
                        isClimb = false;
                        playAnimator.ResetTrigger("IsClimb"); //重置触发器  退出
                        playAnimator.SetTrigger("exitClimp");
                    }
                }
                else//如果距离小于  根据方向进行位移修正
                {
                    float tempX = 0;//新的X轴的位置
                    if (dir > 0)
                    {
                        tempX = tempXVaule - boxSize.x * 0.5f - distance + 0.05f; //多加上0.05f的修正距离，防止出现由于精度问题产生的鬼畜行为
                    }
                    else
                    {
                        tempX = tempXVaule + boxSize.x * 0.5f + distance - 0.05f;
                    }
                    if (isSSState)
                    {
                        isSSState = false;  //如果是超级冲刺撞到了墙，结束冲刺
                        AudioManger.Instance.DeleteClipObj(1);
                        AudioManger.Instance.PlayAudio("hero_super_dash_impact_wall", transform.position);
                        StartCoroutine(SprintSlowdown(false));
                    }
                    transform.position = new Vector3(tempX, transform.position.y, 0);//修改玩家的位置
                    if (lRHit2D.collider.CompareTag("Untagged") || lRHit2D.collider.CompareTag("Arc"))    //如果左右是墙或者天花板
                    {
                        EnterClimpFunc(transform.position); //检测当前是否能够进入爬墙状态
                        playAnimator.ResetTrigger("exitClimp");
                    }
                    else
                    {
                        Die();
                    }

                }

            }
            else
            {
                transform.position += new Vector3(moveDistance.x, 0, 0);
                if (isClimb)
                {
                    isClimb = false;
                    playAnimator.SetTrigger("exitClimp");
                    playAnimator.ResetTrigger("IsClimb"); //重置触发器  退出
                }

            }
        }
        else
        {
            if (isClimb)    //当左右速度无值时且处于爬墙状态时
            {
                ExitClimpFunc();
            }
        }
        //更新方向信息，上下轴
        if (moveSpeed.y > 0)
        {
            dir = 1;
        }
        else if (moveSpeed.y < 0)
        {
            dir = -1;
        }
        else
        {
            dir = 0;
        }
        //上下方向进行判断
        if (dir != 0)
        {
            RaycastHit2D uDHit2D = Physics2D.BoxCast(transform.position, boxSize, 0, Vector3.up * dir, 5.0f, playerLayerMask);
            if (uDHit2D.collider != null)
            {
                float tempYVaule = (float)Math.Round(uDHit2D.point.y, 1);
                Vector3 colliderPoint = new Vector3(transform.position.x, tempYVaule);
                float tempDistance = Vector3.Distance(transform.position, colliderPoint);

                if (tempDistance > (boxSize.y * 0.5f + distance))
                {
                    float tempY = 0;
                    float nextY = transform.position.y + moveDistance.y;
                    if (dir > 0)
                    {
                        tempY = tempYVaule - boxSize.y * 0.5f - distance;
                        if (nextY > tempY)
                        {
                            transform.position = new Vector3(transform.position.x, tempY+0.1f, 0);
                        }
                        else
                        {
                            transform.position += new Vector3(0, moveDistance.y, 0);
                        }
                    }
                    else
                    {
                        tempY = tempYVaule + boxSize.y * 0.5f + distance;
                        if (nextY < tempY)
                        {
                            transform.position = new Vector3(transform.position.x, tempY-0.1f, 0); //上下方向多减少0.1f的修正距离，防止鬼畜
                        }
                        else
                        {
                            transform.position += new Vector3(0, moveDistance.y, 0);
                        }
                    }
                    isGround = false;   //更新在地面的bool值
                }
                else
                {
                    float tempY = 0;
                    if (dir > 0)//如果是朝上方向移动，且距离小于规定距离，就说明玩家头上碰到了物体，反之同理。
                    {
                        tempY = uDHit2D.point.y - boxSize.y * 0.5f - distance + 0.05f;
                        isGround = false;
                        Debug.Log("头上碰到了物体");
                    }
                    else
                    {
                        tempY = uDHit2D.point.y + boxSize.y * 0.5f + distance - 0.05f;
                        Debug.Log("着地");
                        AudioManger.Instance.PlayAudio("hero_land_soft", transform.position);
                        isGround = true;
                    }
                    moveSpeed.y = 0;
                    transform.position = new Vector3(transform.position.x, tempY, 0);
                    if (uDHit2D.collider.CompareTag("Trap"))    //如果头上是陷阱  死亡
                    {
                        Die();
                    }
                    else if (uDHit2D.collider.CompareTag("Bramble"))
                    {
                        Die();
                    }
                }
            }
            else
            {
                isGround = false;
                transform.position += new Vector3(0, moveDistance.y, 0);
            }
        }
        else
        {
            isGround = CheckIsGround();//更新在地面的bool值
        }
    }

    /// <summary>
    /// 进入爬墙的函数
    /// </summary>
    public void EnterClimpFunc(Vector3 rayPoint)
    {
        //设定碰到墙 且  从碰撞点往下 玩家碰撞盒子高度内  没有碰撞体  就可进入碰撞状态。
        RaycastHit2D hit2D = Physics2D.BoxCast(rayPoint, boxSize, 0, Vector2.down, boxSize.y, playerLayerMask);
        if (hit2D.collider != null)
        {
            Debug.Log("无法进入爬墙状态  "+ hit2D.collider.name);
        }
        else
        {
            //如果上方是异形碰撞体，那么就无法进入爬墙状态
            hit2D = Physics2D.BoxCast(rayPoint, boxSize, 0, Vector2.up, boxSize.y*0.8f, playerLayerMask);
            if (hit2D.collider == null || hit2D.collider.gameObject.tag != "Arc")
            {
                playAnimator.SetTrigger("IsClimb");//动画切换
                isClimb = true;
                isCanSprint = true; //爬墙状态，冲刺重置
            }
        }
    }

    /// <summary>
    /// 退出爬墙状态检测
    /// </summary>
    public void ExitClimpFunc()
    {
        RaycastHit2D hit2D = new RaycastHit2D();
        switch (nowDir)
        {
            case PlayDir.Left:
                hit2D = Physics2D.Raycast(transform.position, Vector3.left, boxSize.x);
                break;
            case PlayDir.Right:
                hit2D = Physics2D.Raycast(transform.position, Vector3.right, boxSize.x);
                break;
        }

        if (hit2D.collider == null)
        {
            isClimb = false;
            playAnimator.SetTrigger("exitClimp");
            playAnimator.ResetTrigger("IsClimb"); //重置触发器  退出
        }
    }

    /// <summary>
    /// 显示盒子射线
    /// </summary>
    public void DrawBoxLine(Vector3 point,Vector2 size,float time=0)
    {
        float x, y;
        x = point.x;
        y = point.y;
        float m, n;
        m = size.x;
        n = size.y;

        Vector3 point1, point2, point3, point4;
        point1 = new Vector3(x - m * 0.5f, y + n * 0.5f, 0);
        point2 = new Vector3(x + m * 0.5f, y + n * 0.5f, 0);
        point3 = new Vector3(x + m * 0.5f, y - n * 0.5f, 0);
        point4 = new Vector3(x - m * 0.5f, y - n * 0.5f, 0);

        Debug.DrawLine(point1, point2, Color.red, time);
        Debug.DrawLine(point2, point3, Color.red, time);
        Debug.DrawLine(point3, point4, Color.red, time);
        Debug.DrawLine(point4, point1, Color.red, time);
    }

    /// <summary>
    /// 初始化超级冲刺使用的特效物体
    /// </summary>
    public void InitSupreObjs()
    {
        particleObjs = new List<GameObject>();
        particleObj = transform.Find("SuperSprint").gameObject;
        GameObject tempObjs = particleObj.transform.Find("one").gameObject;
        particleObjs.Add(tempObjs);
        tempObjs = particleObj.transform.Find("two").gameObject;
        particleObjs.Add(tempObjs);
        tempObjs = particleObj.transform.Find("three").gameObject;
        particleObjs.Add(tempObjs);
        tempObjs = particleObj.transform.Find("four").gameObject;
        particleObjs.Add(tempObjs);
        tempObjs = particleObj.transform.Find("five").gameObject;
        particleObjs.Add(tempObjs);
        tempObjs = particleObj.transform.Find("six").gameObject;
        particleObjs.Add(tempObjs);
        //初始化用于完成的特效物体
        effectObjs = new List<GameObject>();
        effectComplate = transform.Find("SuperEffectOne").gameObject;
        tempObjs = effectComplate.transform.Find("one").gameObject;
        effectObjs.Add(tempObjs);
        tempObjs = effectComplate.transform.Find("two").gameObject;
        effectObjs.Add(tempObjs);
        tempObjs = effectComplate.transform.Find("three").gameObject;
        effectObjs.Add(tempObjs);
        tempObjs = effectComplate.transform.Find("four").gameObject;
        effectObjs.Add(tempObjs);
        //初始化拖尾物体相关
        superTrailing = transform.Find("SuperTrailing").gameObject;
        superTrailing.SetActive(false);
    }

    bool isPlayState = false;

    /// <summary>
    /// 播放超级冲刺特效（闪光）
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayEffectSuper()
    {
        for (int i=0;i< effectObjs.Count;i++)
        {
            if (i == 1)
            {
                effectObjs[i].SetActive(true);
            }
            else
            {
                if (i!=0)
                {
                    if (i==2)
                    {
                        effectObjs[i - 2].SetActive(false);
                    }
                    effectObjs[i-1].SetActive(false);
                }
                effectObjs[i].SetActive(true);
            }
            yield return null;
            yield return null;
        }

        for (int i = 0; i < effectObjs.Count; i++)
        {
            effectObjs[i].SetActive(false);
        }
    }

    public bool isPlayAudioState;
    /// <summary>
    /// 播放行走声音
    /// </summary>
    /// <param name="h"></param>
    public void PlayRunAudio(float h)
    {
        if (playSource==null)
        {
            InitRunAudioObj();
        }
        if (h != 0 && isGround && !isPlayAudioState)
        {
            isPlayAudioState = true;
            playSource.Play();
        }
        else if(h==0 || !isGround)
        {
            playSource.Stop();
            isPlayAudioState = false;
        }
    }

    AudioSource playSource;
    /// <summary>
    /// 初始化玩家脚步声音播放物体
    /// </summary>
    public void InitRunAudioObj()
    {
        playSource = AudioManger.Instance.PlayAudio("hero_run_footsteps_stone", transform, 2);
        playSource.Stop();
    }
}
