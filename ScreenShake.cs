using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{

    static public float defaultshakecoefficient = 0.01f;
    static public float shakecoefficient = 0.01f;

    static private float stoptime;
    static private bool shake;
    static private int shakeFrame = 0;

    private bool isshaked = false;

    private float curtime;

    private Transform maincamera;
    private Vector3 camstartpos, shakepos;

    // Use this for initialization
    void Awake()
    {
        stoptime = 0f;
        maincamera = transform;
        camstartpos = maincamera.position;
    }

    // Update is called once per frame
    void Update()
    {
        curtime = Time.realtimeSinceStartup;
        if (shakeFrame > 0)
        {
            Shake();
            shakeFrame--;
            if (shakeFrame == 0)
            {
                maincamera.position = camstartpos;
            }
        }
        //if (shake)
        //{
        //    if (curtime > stoptime)
        //    {
        //        if (isshaked)
        //        {
        //            Time.timeScale = 1f;
        //            maincamera.position = camstartpos;
        //            shake = false;
        //            isshaked = false;
        //        }
        //        else
        //        {
        //            Shake(); //isshaked = true in this function
        //        }
        //    }
        //    else
        //    {
        //        Time.timeScale = 0.1f;
        //        Shake(); //isshaked = true in this function
        //    }
        //}
    }

    void Shake()
    {
        shakepos = Random.insideUnitSphere.normalized * Random.Range(0.8f, 1f) * shakecoefficient;
        //Debug.Log(shakepos.ToString("f4"));
        //shakepos.z = -10f; // z from camstartpos
        //Debug.Log("shake " + maincamera.position);
        maincamera.position = camstartpos + shakepos;
        isshaked = true;
        //Debug.Log("shake "+ maincamera.position);
    }

    public void UpdateCameraPos(Vector3 newPos)
    {
        camstartpos = newPos;
        Time.timeScale = 1f;
        maincamera.position = camstartpos;
        shake = false;
        isshaked = false;
    }

    static public void StopScreen(float time)
    {
        ShakeScreen(shakecoefficient);
        stoptime = Time.realtimeSinceStartup + time;
    }

    static public void StopScreen(float time, float coefficient)
    {
        ShakeScreen(coefficient);
        stoptime = Time.realtimeSinceStartup + time;
    }

    static public void ShakeScreen(float coefficient)
    {
        shake = true;
        shakecoefficient = coefficient;
    }

    static public void ShakeFrame(float coefficient, int frameAmount)
    {
        //shake = true;
        shakeFrame = frameAmount;
        shakecoefficient = coefficient;
    }

    static public void ShakeScreen()
    {
        shake = true;
        shakecoefficient = defaultshakecoefficient;
    }

    public void UpdateCamPos()
    {
        camstartpos = maincamera.position;
    }
}
