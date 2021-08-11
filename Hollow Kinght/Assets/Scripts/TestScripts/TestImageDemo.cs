using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestImageDemo : MonoBehaviour {
    string nama = "atlas0 #50600357_";
    public Transform parentTrs;
    public SpriteRenderer spriteTelpate;
    public float speed;
    bool isMove;
    public GameObject obj;
    //public Transform camraTrans;
    void Start () {
        StartCoroutine(CreatSpriteObj());
	}
	
	// Update is called once per frame
	void Update () {
        if (isMove)
        {
            Move();
        }
	}

    IEnumerator CreatSpriteObj()
    {
        yield return new WaitForSeconds(0.2f);
        obj.SetActive(false);
        int count = 0;
        float startY = 4.4f;
        float startX = -8.2f;
        Sprite[] tempSprites = Resources.LoadAll<Sprite>("atlas0 #050600357");
        Quaternion tempQua = Quaternion.Euler(0, 180, 90);
        for (int i = 1; i < tempSprites.Length; i++)
        {
            Sprite hollowSprite = tempSprites[i] as Sprite;

            if (hollowSprite != null)
            {
                Vector3 creatPoint = new Vector3(startX, startY, 0);
                GameObject tempObj = Instantiate(spriteTelpate.gameObject, creatPoint, Quaternion.identity);
                tempObj.transform.SetParent(parentTrs);
                SpriteRenderer tempImage = tempObj.GetComponent<SpriteRenderer>();
                tempImage.sprite = hollowSprite;
                count++;
                tempObj.name = "Sprite_" + (i - 1).ToString();
                if (hollowSprite.rect.width > 100)
                {
                    tempObj.transform.rotation *= tempQua;
                }
                startX = startX + 1.5f;
                if (count > 20)
                {
                    count = 0;
                    startX = -8.2f;
                    startY -= 1.6f;

                }
                //yield return null;
            }
        }


        yield return new WaitForSeconds(0.1f);
        isMove = true;
    }

    public void Move()
    {
        transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
    }
}
