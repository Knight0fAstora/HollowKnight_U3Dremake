using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour {

    [Header("多少时间后销毁")]
    public float destroyTime;
    float time;
    void Start () {
        time = 0;

    }
	
	// Update is called once per frame
	void Update ()
    {

        time += Time.deltaTime;
        if (time>destroyTime)
        {
            Destroy(gameObject);
        }

    }
}
