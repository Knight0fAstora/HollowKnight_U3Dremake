using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAsyncScene : MonoBehaviour {

    // 目标进度
    float target = 0;
    // 读取场景的进度，取值范围0~1
    float progress = 0;
    // 异步对象
    AsyncOperation op = null;

    void Start()
    {
        Debug.Log("开始LoadScene");

        op = SceneManager.LoadSceneAsync("Level1");
        op.allowSceneActivation = false;

        // 开启协程，开始调用加载方法
        StartCoroutine(processLoading());
    }

    bool isCompelate = false;
    // 加载进度
    IEnumerator processLoading()
    {
        while (!isCompelate)
        {
            target = op.progress; // 进度条取值范围0~1
            if (target >= 0.9f)
            {
                target = 1;
                isCompelate = true;
            }
            yield return 0;
        }

        yield return new WaitForSeconds(1f);
        op.allowSceneActivation = true;
    }
}
