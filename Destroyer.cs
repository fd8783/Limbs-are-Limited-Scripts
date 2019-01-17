using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    public bool autoDestroy = true;
    public float destroyTime = 0.5f;

	// Use this for initialization
	void Start ()
    {
        if (autoDestroy)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    public void DestroyThis(float time)
    {
        Destroy(gameObject, time);
    }
}
