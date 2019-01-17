using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToNextLevel : MonoBehaviour {

    private bool enable = false;


    private void OnEnable()
    {
        enable = true;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            BackgroundSetting.Instance.NextLevel();
            gameObject.SetActive(false);
        }
	}
}
