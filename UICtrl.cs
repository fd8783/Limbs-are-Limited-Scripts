using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class UICtrl : MonoBehaviour {

    private GameObject livePlane;

    //init
    private PostProcessVolume postProcess;
    private Vignette vig = null;

    //control Vignette
    private float targetVigIntenstiy = 0f, maxVigIntensity = 0.4f, smoothVigIntensityVel = 0f, smoothVigIntensityTime = 1f, defaultSmoothVigIntensityTime = 0.15f;

	// Use this for initialization
	void Awake () {
        postProcess = GetComponent<PostProcessVolume>();
        postProcess.profile.TryGetSettings(out vig);
        if (GameObject.Find("SpawnCtrl"))
            livePlane = GameObject.Find("SpawnCtrl").transform.Find("Plane").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        VigSmooth();
    }

    void VigSmooth()
    {
        if (Mathf.Abs(vig.intensity.value - targetVigIntenstiy) > 0.01f)
        {
            vig.intensity.value = Mathf.SmoothDamp(vig.intensity.value, targetVigIntenstiy, ref smoothVigIntensityVel, smoothVigIntensityTime);
        }
    }

    public void VigOnOff(bool isOn)
    {
        smoothVigIntensityTime = Time.timeScale * defaultSmoothVigIntensityTime;
        targetVigIntenstiy = isOn ? maxVigIntensity : 0f;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MoveToScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void LoadLevel(int levelNum)
    {
        BackgroundSetting.Instance.LoadLevel(levelNum);
    }

    public void EnableLive()
    {
        BackgroundSetting.haveLive = !BackgroundSetting.haveLive;
        if (livePlane != null)
            livePlane.SetActive(BackgroundSetting.haveLive);
    }
}
