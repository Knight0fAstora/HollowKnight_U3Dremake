using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {

    public bool isUse;
    Transform player;
    public Transform canvasUI;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canvasUI = GameObject.Find("Canvas").transform;

    }
	
	// Update is called once per frame
	void Update () {
        if (!isUse)
        {
            UpdateSavePoint();
        }
	}

    public void UpdateSavePoint()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance<1.0)
        {
            MoveController tempController = player.GetComponent<MoveController>();
            tempController.startPoint = transform.position;
            var saveUi = Resources.Load<GameObject>("Prefebs/SaveUI");
            AudioManger.Instance.PlayAudio("ui_save",player.transform.position);
            GameObject tempSaveUi = Instantiate(saveUi, canvasUI);
            var desSc = tempSaveUi.AddComponent<EffectDestroy>();
            desSc.destroyTime = 0.6f;
            //saveUi.transform.SetParent(canvasUI);
            isUse = true;
        }
    }
}
