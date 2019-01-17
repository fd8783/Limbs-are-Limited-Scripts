using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButCtrl : MonoBehaviour {

    public Transform target;

    public float moveTime = 0.1f, recoverTime = 3f;
    public Vector3 offSet;

    private bool moving = false;
    private Vector3 startPos, targetPos, movingTargetPos, smoothMoveVel = Vector3.zero;
    private float recoverTimeCounter = 1;
    private float moveEndTime;

    // Use this for initialization
    void Start () {
        startPos = target.position;
        targetPos = startPos + offSet;
    }
	
	// Update is called once per frame
	void Update () {
		if (moving)
        {
            if (Time.time < moveEndTime)
            {
                target.position = Vector3.SmoothDamp(target.position, targetPos, ref smoothMoveVel, moveTime);
            }
            else
            {
                moving = false;
            }
        }
        else
        {
            if (recoverTimeCounter < 1f)
            {
                recoverTimeCounter += (Time.deltaTime / recoverTime);
                target.position = Vector3.Lerp(targetPos, startPos, recoverTimeCounter);
            }
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == BackgroundSetting.BodyPartLayer || collision.gameObject.layer == BackgroundSetting.DullLayer)
        {
            ButtonTriggered();
        }
    }

    public void ButtonTriggered()
    {
        moving = true;
        moveEndTime = Time.time + moveTime+0.5f;
        recoverTimeCounter = 0f;
    }
}
