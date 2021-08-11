using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadSpriteGrid : MonoBehaviour {

    string nama = "atlas0 #50600357_";
    public Sprite spriteHollow;
    int count = 0;
    public Transform parentrans;
    public SpriteRenderer spriteTelpate;
    Dictionary<int, SpriteRenderer> dictSprite;
    List<int> spriteRemoveId;
	void Start () {
        //string pathName = nama + count.ToString();
        //spriteRemoveId = new List<int>() {15,29,17,13,14,18,24,4,16 };
        //object[] tempSprite = Resources.LoadAll("atlas0 #050600357");

        //if (tempSprite!=null)
        //{
        //    //var xx = tempSprite[1];
        //    //int i = 0;
        //    dictSprite = new Dictionary<int, SpriteRenderer>();
        //    StartCoroutine(CreatSpriteObj(tempSprite));
        //}
        //else
        //{
        //    Debug.Log("读取失败");
        //}
        //AssetDatabase.CreateFolder(Application.dataPath, "测试");
        //Debug.Log("创建完毕"+Application.dataPath);
       
        // File.Create("C:/Users/Administrator/Desktop/空洞骑士动作序列/测试2.txt");
        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CreatSpriteObj(object[] tempSprites)
    {
        int count = 0;
        int startY = 8;
        int startX = -12;
        Quaternion tempQua = Quaternion.Euler(0, 180, 90);
        for (int i = 1; i < tempSprites.Length; i++)
        {
            Sprite hollowSprite = tempSprites[i] as Sprite;

            if (hollowSprite != null)
            {
                Vector3 creatPoint = new Vector3(++startX, startY, 0);
                GameObject tempObj = Instantiate(spriteTelpate.gameObject, creatPoint, Quaternion.identity);
                tempObj.transform.SetParent(parentrans);
                SpriteRenderer tempImage = tempObj.GetComponent<SpriteRenderer>();
                tempImage.sprite = hollowSprite;
                count++;
                tempObj.name = "Sprite_" + (i-1).ToString();
                if (hollowSprite.rect.width>100)
                {
                    tempObj.transform.rotation *= tempQua;
                }
                if (count>24)
                {
                    count = 0;
                    startX = -12;
                    startY -= 2;

                }
                dictSprite.Add(i-1, tempImage);
                yield return new WaitForSeconds(0.1f);
            }
        }

        for (int i=0;i<spriteRemoveId.Count;i++)
        {
            SpriteRenderer tempSpriteRender = dictSprite[spriteRemoveId[i]];
            tempSpriteRender.color = new Color32(0, 0, 0, 255);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    //[MenuItem("Tools/SpriteSplit")]
    static void SpriteSlice()
    {
        //Texture2D image = Selection.activeObject as Texture2D; // 得到选中的图集 该图集事先已经被修改属性
        //string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image)); //文件所在的路径
        //string path = rootPath + "/" + image.name + ".PNG"; //Assets/Resources/UI/XXX.png
        //TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
        //// AssetDatabase.CreateFolder(rootPath, image.name);
        //string defaultPath = "E:/空洞骑士序列帧文件/";
        //// 遍历图集中每一张小图,并使用文件IO写入到同级目录的文件夹中
        //int i = 0;

        //foreach (SpriteMetaData metaData in texImp.spritesheet)
        //{
        //    Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

        //    for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)
        //    {
        //        for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
        //            myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
        //    }
        //    if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
        //    {
        //        Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
        //        newTexture.SetPixels(myimage.GetPixels(0), 0);
        //        myimage = newTexture;
        //    }
        //    var pngData = myimage.EncodeToPNG();

        //    string tempPath = defaultPath + i.ToString()+".PNG";

        //    File.WriteAllBytes(tempPath, pngData);
        //    i++;
        //    //AssetDatabase.Refresh();
        }
    }

    


