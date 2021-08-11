using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShadowHalo : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public int framCount;      //多少帧后消失
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(HideHalo());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator HideHalo()
    {


        float aVaule = 255;
        float tempVaule = 0;
        tempVaule = 1.0f / (float)framCount;

        for (int i=0;i<framCount;i++)
        {
            yield return null;
            aVaule -= 255.0f * tempVaule;
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            spriteRenderer.color = new Color32(255, 255, 255, (byte)aVaule);

        }
    }
}
