using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour {

    public bool isUse;
    Transform player;
    public Transform canvasUI;
    public LevelUIRoot root;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canvasUI = GameObject.Find("Canvas").transform;
        root = canvasUI.GetComponent<LevelUIRoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUse)
        {
            UpdateSavePoint();
        }
    }

    public void UpdateSavePoint()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < 2.0)
        {
            root.OpenEndGameUI();
            isUse = true;
        }
    }
}
