using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class keyUISetting : MonoBehaviour {

    public Transform groupTrans;
    public Transform restTrans;
    public Transform returnTrans;
    public GameObject keySetTemplate;       //按键设置的模板物体
    public List<GameObject> keySetings;

	void Start ()
    {
        keySetTemplate = Resources.Load<GameObject>("Prefebs/keySetBtn");
        groupTrans = transform.Find("KeySetGroup");
        restTrans = transform.Find("restBtn");
        returnTrans = transform.Find("returnBtn");
        keySetings = new List<GameObject>();
        InitSetBtn();
        InitGroup();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

    }
    public void InitSetBtn()
    {
        AddUIEvent(restTrans);
        AddUIEvent(returnTrans);
        var tempKeyClickOne = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(KeyInitClick, EventTriggerType.PointerDown);
        RegisterEvent(restTrans.gameObject, tempKeyClickOne);
        var tempKeyClickTwo = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(ReturnClick, EventTriggerType.PointerDown);
        RegisterEvent(returnTrans.gameObject, tempKeyClickTwo);
    }

    public void InitGroup()
    {
        string[] keyNames = new string[] {"上","下","左","右","跳跃","冲刺","超级冲刺","菜单" };
        KeyCode[] codes = new KeyCode[] {KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D,KeyCode.Space,KeyCode.J,KeyCode.L,KeyCode.Escape};
        for (int i=0;i< keyNames.Length; i++)
        {
            GameObject tempSetBtn = Instantiate(keySetTemplate, groupTrans);
            AddUIEvent(tempSetBtn.transform);
           InitBtnData(tempSetBtn.transform,keyNames[i], codes[i]);
            keySetings.Add(tempSetBtn);
        }
    }

    public void InitBtnData(Transform btnTrans,string keyName,KeyCode code)
    {
        Text nameText = btnTrans.Find("keyName").GetComponent<Text>();
        nameText.text = keyName;
        int codeNumber = (int)code;
        string keyVaule = KeyCodeToKeyName(code);
        Transform lookKeyUiShort = btnTrans.Find("keyFrameShot");
        Transform lookKeyUiLong = btnTrans.Find("keyFrameLong");
        if (codeNumber >= 97 && codeNumber <= 122)
        {
            lookKeyUiLong.gameObject.SetActive(false);
            Text keyText = lookKeyUiShort.Find("ketText").GetComponent<Text>();
            keyText.text = keyVaule;
        }
        else
        {
            lookKeyUiShort.gameObject.SetActive(false);
            Text keyText = lookKeyUiLong.Find("ketText").GetComponent<Text>();
            keyText.text = keyVaule;
        }
        GameObject inputTips = btnTrans.Find("setText").gameObject;
        inputTips.SetActive(false);
    }

    public void KeyClickEvent(BaseEventData data)
    {

    }

    public void KeyExitEvent(BaseEventData btnTrans)
    {

    }

    

    public string KeyCodeToKeyName(KeyCode code)
    {
        string codeStr = code.ToString();
        var strs = codeStr.Split('.');
        return strs[strs.Length - 1];
    }

    public EventTrigger UIAddTrigger(GameObject uIObj, UnityAction<BaseEventData> action, EventTriggerType triggerType)
    {
        EventTrigger trigger = uIObj.GetComponent<EventTrigger>();
        if (trigger==null)
        {
            trigger = uIObj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = triggerType;
        enterEntry.callback.AddListener(action);
        trigger.triggers.Add(enterEntry);
        return trigger;
    }
    /// <summary>
    /// 添加UI事件
    /// </summary>
    /// <param name="trans"></param>
    public void AddUIEvent(Transform trans)
    {
        IconMove[] moves = InitMaskMove(trans);
        UnityAction<BaseEventData> funcEnter = CreatEvent(moves, 1);
        UnityAction<BaseEventData> funcExit = CreatEvent(moves, 2);
        var tempKeyEnter = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(funcEnter,EventTriggerType.PointerEnter);
        var tempKeyExit = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(funcExit, EventTriggerType.PointerExit);
        RegisterEvent(trans.gameObject, tempKeyEnter, tempKeyExit);
    }

    /// <summary>
    /// 注册UI事件
    /// </summary>
    public void RegisterEvent(GameObject uIObj,params KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>[] events)
    {
        var trigger = uIObj.AddComponent<EventTrigger>();
        if (events!=null)
        {
            for (int i = 0; i < events.Length; i++)
            {
                EventTrigger.Entry tempEntry = new EventTrigger.Entry();
                tempEntry.eventID = events[i].Value;
                tempEntry.callback.AddListener(events[i].Key);
                trigger.triggers.Add(tempEntry);
            }
        }

    }

    public IconMove[] InitMaskMove(Transform trans)
    {
        IconMove[] moves = new IconMove[2];
        GameObject leftIcon = trans.Find("Masks/LeftMask/leftImage").gameObject;
        GameObject rightIcon = trans.Find("Masks/RightMask/rightImage").gameObject;
        IconMove leftMove = leftIcon.AddComponent<IconMove>();
        leftMove.Init(0);
        IconMove rightMove = rightIcon.AddComponent<IconMove>();
        rightMove.Init(1);
        moves[0] = leftMove;
        moves[1] = rightMove;
        return moves;
    }

    /// <summary>
    /// 模式参数 1为鼠标进入 2为鼠标退出
    /// </summary>
    /// <returns></returns>
    public UnityAction<BaseEventData> CreatEvent(IconMove[] moves,int mode)
    {
        UnityAction<BaseEventData> tempEvent = new UnityAction<BaseEventData>((data)=> 
        {
            for (int i=0;i<moves.Length;i++)
            {
                if (mode == 1)
                {
                    moves[i].MouseEnterClick();
                }
                else
                {
                    moves[i].MouseExitClick();
                }

            }
        });
        return tempEvent;
        
    }

    public void KeyInitClick(BaseEventData data)
    {
        InputManager.Instance.KeyInit();
       // RefreshData();
    }

    public void ReturnClick(BaseEventData data)
    {
        UIRoot root = transform.parent.GetComponent<UIRoot>();
        root.CloseKeySetting();
    }

    /// <summary>
    /// 刷新面版数据
    /// </summary>
    public void RefreshData(Text setText,Transform btnTrans)
    {
        Transform lookKeyUiShort = btnTrans.Find("keyFrameShot");
        Transform lookKeyUiLong = btnTrans.Find("keyFrameLong");
        if (InputManager.Instance.nowDownKey != KeyCode.KeypadEnter)
        {
            string keyName = KeyCodeToKeyName(InputManager.Instance.nowDownKey);
            setText.text = keyName;
        }
        else
        {
            string codeName = "KeyCode." + setText.text;
            KeyCode tempKey = (KeyCode)Enum.Parse(typeof(KeyCode), codeName);

            int codeNumber = (int)tempKey;
            if (codeNumber >= 97 && codeNumber <= 122)
            {
                lookKeyUiLong.gameObject.SetActive(false);
                Text keyText = lookKeyUiShort.Find("ketText").GetComponent<Text>();
                keyText.text = setText.text;
            }
            else
            {
                lookKeyUiShort.gameObject.SetActive(false);
                Text keyText = lookKeyUiLong.Find("ketText").GetComponent<Text>();
                keyText.text = setText.text;
            }
        }
    }
}
