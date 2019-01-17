using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCannon : MonoBehaviour {

    public CannonCtrl cannonScript;

	// Use this for initialization
	void Awake () {
        cannonScript = GetComponent<CannonCtrl>();

    }
	
	// Update is called once per frame
	void Update () {
        //cannonScript.Aim();
        if (Input.GetMouseButtonUp(0))
        {
            cannonScript.FireEffect();
        }
	}
}
