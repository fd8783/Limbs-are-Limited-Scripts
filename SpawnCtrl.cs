using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCtrl : MonoBehaviour {

    public Transform spawnPlayer;

    private CharacterCtrl curPlayerScript;
    private AudioSource spawnSound;
    private Transform liveCam;

    public MeshRenderer planeMesh;
    public Material livePlaneMat, deadPlaneMat;
    private bool playerLiving = true;

	// Use this for initialization
	void Awake ()
    {
        spawnSound = GetComponent<AudioSource>();
        liveCam = transform.Find("LiveCamera");
        transform.Find("Plane").gameObject.SetActive(BackgroundSetting.haveLive);
        Spawn();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
        {
            Spawn();
        }
        LiveCamFollow();

    }

    void Spawn()
    {
        if (curPlayerScript != null && curPlayerScript.enabled)
        {
            curPlayerScript.Death();
        }
        curPlayerScript = Instantiate(spawnPlayer, transform.position, spawnPlayer.rotation).GetComponent<CharacterCtrl>();
        if (spawnSound.isPlaying)
        {
            spawnSound.Stop();
        }
        spawnSound.pitch = Random.Range(0.8f, 1.1f);
        spawnSound.volume = Random.Range(0.7f, 1f);
        spawnSound.Play();
    }

    void LiveCamFollow()
    {
        if (curPlayerScript.enabled != playerLiving)
        {
            playerLiving = !playerLiving;
            planeMesh.material = playerLiving ? livePlaneMat : deadPlaneMat;
        }
        liveCam.position = curPlayerScript.bodyPart[0].transform.position + new Vector3(0, -0.1f, -50f);
        //liveCam.localRotation = Quaternion.Euler(0, 180f, 0);
    }
}
