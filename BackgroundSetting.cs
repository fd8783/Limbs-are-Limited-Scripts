using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundSetting : MonoBehaviour {

    public static BackgroundSetting Instance;

    public static int PlayerLayer = 9, GroundLayer = 12, BodyPartLayer = 13, DullLayer = 14, ObstacleLayer = 15;

    public static int totalLevel = 13, curLevel = 1, highestLevel = 1;   

    public static Material highlightMat, bloodRedMat;

    public static bool haveLive = false;

    public GameObject gameJoltUI;

    // Use this for initialization
    void Awake () {
        if (Instance == null)
        {
            Instance = this;
            highlightMat = Resources.Load<Material>("Material/Highlight");
            bloodRedMat = Resources.Load<Material>("Material/bloodRed");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (gameJoltUI.activeSelf) return;
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Application.Quit();
            else
                SceneManager.LoadScene(0);
        }
    }

    public void LoadLevel(int levelNum)
    {
        curLevel = levelNum;
        if (curLevel > highestLevel)
        {
            highestLevel = curLevel;
        }
        SceneManager.LoadScene(levelNum + 1);   //level 1 in sence num 2, 0 = Menu, 1 = level selection
    }

    public void NextLevel()
    {
        if (curLevel == totalLevel)
        {
            Debug.LogWarning("Next Level is being called when it's last level");
            return;
        }
        LoadLevel(curLevel + 1);
    }
}
