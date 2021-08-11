using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager Instance;

    public bool isGetKey;
    public KeyCode nowDownKey;  //现在按下的键

    [Header("控制是否使用自定义按键")]
    public bool keyIsSet;
    [Header("上")]
    public KeyCode moveUpKey;
    [Header("下")]
    public KeyCode moveDownKey;
    [Header("左")]
    public KeyCode moveLeftKey;
    [Header("右")]
    public KeyCode moveRightKey;
    [Header("攻击按键")]
    public KeyCode attackKey;
    [Header("跳跃按键")]
    public KeyCode jumpKey;
    [Header("冲刺按键")]
    public KeyCode sprintKey;
    [Header("超级冲刺按键")]
    public KeyCode superKey;
    [Header("菜单按键")]
    public KeyCode menuKey;
    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        KeyInit();
        isGetKey = false;
    }


    public void KeyInit()
    {
        moveUpKey = KeyCode.W;
        moveDownKey = KeyCode.S;
        moveLeftKey = KeyCode.A;
        moveRightKey = KeyCode.D;
        attackKey = KeyCode.K;
        jumpKey = KeyCode.Space;
        sprintKey = KeyCode.J;
        superKey = KeyCode.L;
        menuKey = KeyCode.Escape;
    }

    private void OnGUI()
    {
        if (isGetKey)
        {
            if (Input.anyKeyDown)
            {
                Event e = Event.current;
                if (e.isKey)
                {
                    if (e.keyCode != KeyCode.KeypadEnter && e.keyCode != KeyCode.None)
                    {
                        nowDownKey = e.keyCode;
                    }

                }
            }
        }
    }
}
