using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonLoad : MonoBehaviour {

    private Button[] buttonList;
    private int buttonCount, playerHighestLevel;

	// Use this for initialization
	void Start () {
        buttonCount = transform.childCount;
        buttonList = new Button[buttonCount];
        playerHighestLevel = BackgroundSetting.highestLevel;
        if (BackgroundSetting.totalLevel != buttonCount)
        {
            Debug.LogWarning("button in level selection not match with total level in BGsetting");
        }

        for (int i = 0; i < playerHighestLevel; i++)
        {
            buttonList[i] = transform.GetChild(i).GetComponent<Button>();
            buttonList[i].interactable = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UnlockAllLevel()
    {
        if (BackgroundSetting.totalLevel != buttonCount)
        {
            Debug.LogWarning("button in level selection not match with total level in BGsetting");
        }

        if (BackgroundSetting.highestLevel == buttonCount)
            return;

        BackgroundSetting.highestLevel = buttonCount-1;  //BackgroundSetting.totalLevel;

        playerHighestLevel = BackgroundSetting.highestLevel;

        for (int i = 0; i < playerHighestLevel; i++)
        {
            buttonList[i] = transform.GetChild(i).GetComponent<Button>();
            buttonList[i].interactable = true;
        }
    }
}
