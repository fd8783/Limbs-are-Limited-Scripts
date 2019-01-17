using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbPuller : MonoBehaviour {

    private Vector3 mouseScreenPos;
    private Ray mouseRay;
    private RaycastHit mousetHit;
    private Transform pullingLimb;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckMouseInput();

    }

    void CheckMouseInput()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out mousetHit, 100f, 1<< BackgroundSetting.BodyPartLayer))
        {
            pullingLimb = mousetHit.transform;
            Debug.DrawRay(mousetHit.point, Vector3.up*100f, Color.black);
        }
    }
}
