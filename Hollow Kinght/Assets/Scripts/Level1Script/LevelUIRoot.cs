using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelUIRoot : MonoBehaviour {

    public List<Button> btnOBjs;
    Button continueBtn;
    Button restBtn;
    Button exitBtn;

    GameObject endUI;
    Text timeText;
    GameObject menuUI;

    bool isOpenMenu;
    bool gameIsEnd;     //游戏是否结束
    private void Awake()
    {
        Application.targetFrameRate = 60;   //设置帧率
    }

    void Start()
    {

        Init();
        isOpenMenu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InputManager.Instance.menuKey) && !isOpenMenu)
        {
            OpenMenu();
            isOpenMenu = true;
        }
    }

    public void Init()
    {
        btnOBjs = new List<Button>();
        endUI = transform.Find("GameEndUI").gameObject;
        timeText = endUI.transform.Find("ExitUIText").GetComponent<Text>();
        endUI.SetActive(false);
        menuUI = transform.Find("MenuUI").gameObject;
        continueBtn = transform.Find("MenuUI/ButtonObjs/ContinueBtn").GetComponent<Button>();
        restBtn = transform.Find("MenuUI/ButtonObjs/RestBtn").GetComponent<Button>();
        exitBtn = transform.Find("MenuUI/ButtonObjs/exitBtn").GetComponent<Button>();
        btnOBjs.Add(continueBtn);
        btnOBjs.Add(restBtn);
        btnOBjs.Add(exitBtn);

        for (int i = 0; i < btnOBjs.Count; i++)
        {
            var trigger = btnOBjs[i].gameObject.AddComponent<EventTrigger>();
            RegisterEvent(trigger, btnOBjs[i].gameObject);
        }

        continueBtn.onClick.AddListener(ContinueBtnClick);
        exitBtn.onClick.AddListener(ExitBtnClick);
        restBtn.onClick.AddListener(ResetScene);
        menuUI.SetActive(false);
    }

    /// <summary>
    /// 注册UI事件
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="btnObj"></param>
    public void RegisterEvent(EventTrigger trigger, GameObject btnObj)
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

    public void ContinueBtnClick()
    {
        CloseMenu();
    }

    public void ExitBtnClick()
    {
        Application.Quit();
    }

    public void ResetScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenMenu()
    {
        Time.timeScale = 0;
        menuUI.SetActive(true);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1;
        isOpenMenu = false;
    }

    public void OpenEndGameUI()
    {
        endUI.SetActive(true);
        StartCoroutine(GameExit());
    }

    IEnumerator GameExit()
    {
        for (int i=3;i>=1;i--)
        {
            timeText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        timeText.text = "拜";
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
}
