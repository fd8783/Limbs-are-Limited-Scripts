using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonCtrl : MonoBehaviour {

    //public List<Transform> cols = new List<Transform>();

    //init
    public bool manLoaded = false;
    public ParticleSystem[] fireDust;
    private CharacterCtrl manScript; 
    private Transform shootPt, loadedMan;
    private AudioSource fireSound;
    //private CannonManager managerScript;
    //private int cannonNum;

    //fire
    public Image powerBarUI;
    private bool charging = false;
    private Vector3 startAimPos, curAimPos;
    private float curPower = 0f;

    // Use this for initialization
    void Awake () {
        shootPt = transform.Find("Model/Shoot");
        fireSound = GetComponent<AudioSource>();
    }

    //public void Settle(CannonManager managerScript, int cannonNum)
    //{
    //    this.managerScript = managerScript;
    //    this.cannonNum = cannonNum;
    //}
	
	// Update is called once per frame
	void Update ()
    {
        Aim();
        if (manLoaded)
        {
            CheckFire();
            UIUpdate();
        }
    }

    public void Aim()
    {
        curAimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curAimPos.z = shootPt.position.z;
        //Debug.Log(Vector3.Distance(curAimPos, shootPt.position));
        Debug.DrawLine(shootPt.position, curAimPos, Color.red);
        shootPt.rotation = Quaternion.LookRotation(curAimPos - shootPt.position, Quaternion.AngleAxis(-90f, transform.right) * (curAimPos - shootPt.position));
    }

    void UIUpdate()
    {
        powerBarUI.fillAmount = curPower;
    }

    void CheckFire()
    {
        curPower = Mathf.Clamp(Vector3.Distance(curAimPos, shootPt.position), 5, 15) / 15f;
        if (!charging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                charging = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                charging = false;
                Fire();
                curPower = 0;
            }
        }
    }

    void Fire()
    {
        manLoaded = false;
        if (manScript.enabled)
        {
            manScript.SetInControl(false);
            manScript.transform.parent = null;
            manScript.OnFly(20000f * curPower);    //power designed by mouse dis
            manScript = null;
        }

        FireEffect();
    }

    public void FireEffect()
    {
        for (int i = 0; i < fireDust.Length; i++)
        {
            fireDust[i].Play();
        }
        ScreenShake.ShakeFrame(0.4f, 8);
        if (fireSound.isPlaying)
        {
            fireSound.Stop();
        }
        fireSound.pitch = Random.Range(0.8f, 1.1f);
        fireSound.volume = Mathf.Max(curPower, 0.6f);
        fireSound.Play();
    }

    private void OnTriggerEnter(Collider col)
    {
        //assume only one player
        if (col.CompareTag("Player"))
        {
            manLoaded = true;
            loadedMan = col.transform;
            manScript = loadedMan.GetComponent<CharacterCtrl>();
            loadedMan.parent = shootPt;
            loadedMan.localPosition = Vector3.zero;
            loadedMan.localRotation = Quaternion.Euler(0f, 90f, 90f);
            manScript.SetInControl(false);
            manScript.LoadedOnCannon();
        }
        //cols.Add(col.transform);
    }

    //private void OnTriggerExit(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        manLoaded = false;
    //        if (manScript != null)
    //        {
    //            manScript.SetInControl(true);
    //            manScript.transform.parent = null;
    //            manScript = null;
    //        }
    //    }
    //    //cols.Remove(col.transform);
    //}
}
