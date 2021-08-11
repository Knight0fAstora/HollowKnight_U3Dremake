using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour {

    public List<Button> btnOBjs;
    Button startBtn;
    Button choiceBtn;
    Button achievementBtn;
    Button extraBtn;
    Button exitBtn;
    public GameObject keyPanle;
    public GameObject btnPanle;
    public GameObject logoObj;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start () {

        Init();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init()
    {
        btnOBjs = new List<Button>();
        startBtn = transform.Find("ButtonObjs/startBtn").GetComponent<Button>();
        choiceBtn = transform.Find("ButtonObjs/choiceBtn").GetComponent<Button>();
        achievementBtn = transform.Find("ButtonObjs/achievementBtn").GetComponent<Button>();
        extraBtn = transform.Find("ButtonObjs/extraBtn").GetComponent<Button>();
        exitBtn = transform.Find("ButtonObjs/exitBtn").GetComponent<Button>();
        keyPanle = transform.Find("keyPanle").gameObject;
        btnPanle = transform.Find("ButtonObjs").gameObject;
        logoObj = transform.Find("LogeImage").gameObject;
        btnOBjs.Add(startBtn);
        btnOBjs.Add(choiceBtn);
        btnOBjs.Add(achievementBtn);
        btnOBjs.Add(extraBtn);
        btnOBjs.Add(exitBtn);

        for (int i=0;i<btnOBjs.Count;i++)
        {
            var trigger = btnOBjs[i].gameObject.AddComponent<EventTrigger>();
            RegisterEvent(trigger,btnOBjs[i].gameObject);
        }

        startBtn.onClick.AddListener(StartBtnClick);
        exitBtn.onClick.AddListener(ExitBtnClick);
        choiceBtn.onClick.AddListener(OpenKeySetting);
    }

    /// <summary>
    /// 注册UI事件
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="btnObj"></param>
    public void RegisterEvent(EventTrigger trigger,GameObject btnObj)
    {
        EventTrigger.Entry enterClick = new EventTrigger.Entry();
        enterClick.eventID = EventTriggerType.PointerEnter;

        EventTrigger.Entry exitClick = new EventTrigger.Entry();
        exitClick.eventID = EventTriggerType.PointerExit;

        GameObject leftIcon = btnObj.transform.Find("Masks/LeftMask/leftImage").gameObject;
        GameObject rightIcon = btnObj.transform.Find("Masks/RightMask/rightImage").gameObject;
        IconMove leftMove = leftIcon.AddComponent<IconMove>();
        leftMove.Init(0);
        IconMove rightMove = rightIcon.AddComponent<IconMove>();
        rightMove.Init(1);

        enterClick.callback.AddListener((o) =>
        {
            leftMove.MouseEnterClick();
            rightMove.MouseEnterClick();
            AudioManger.Instance.PlayAudio("button");
        });

        exitClick.callback.AddListener((o) =>
        {
            leftMove.MouseExitClick();
            rightMove.MouseExitClick();
        });

        trigger.triggers.Add(enterClick);
        trigger.triggers.Add(exitClick);
    }

    public void StartBtnClick()
    {
        SceneManager.LoadScene("Loading");
    }

    public void ExitBtnClick()
    {
        Application.Quit();
    }

    public void OpenKeySetting()
    {
        keyPanle.SetActive(true);
        btnPanle.SetActive(false);
        logoObj.SetActive(false);
    }

    public void CloseKeySetting()
    {
        keyPanle.SetActive(false);
        btnPanle.SetActive(true);
        logoObj.SetActive(true);
    }
}
