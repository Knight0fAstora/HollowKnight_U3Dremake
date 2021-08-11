using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGateCollision : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
