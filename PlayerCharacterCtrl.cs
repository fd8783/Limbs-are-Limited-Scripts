using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCtrl : CharacterCtrl
{
    // get Mouse Input
    private Vector3 mouseScreenPos;
    private Ray mouseRay;
    private RaycastHit mousetHit;
    private Transform pullingLimb;
    private BodyPartCtrl pullingLimbScript;

    //Keyboard Input
    private float inputHorizontal;

    //slow motion
    private UICtrl uiCtrl;
    private bool timeSlowed = false;
    private float slowMotionEndTime = 0f, slowMotionEndSmooth = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        uiCtrl = Camera.main.GetComponent<UICtrl>();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        //Debug.Log(Time.fixedDeltaTime);
        SlowMotionCheck();
    }

    protected override void CheckLimbPulling()
    {
        //Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition),mouseRay.direction*100f,Color.red);
        if (limbPulling)
        {
            if (Input.GetMouseButtonUp(0))
            {
                limbPulling = false;
                if (!pullingLimbScript.separated)
                {
                    pullingLimbScript.BeingPull(Vector3.zero, false);
                }
                pullingLimbScript.Highlight(false);
                pullingLimbScript = null;
                pullingLimb = null;
            }
            else
            {
                pullingLimbScript.Pulling(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f)*0.2f);
            }
        }
        else
        {
            if (!inControl)
            {
                mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(mouseRay, out mousetHit, 100f, 1 << BackgroundSetting.BodyPartLayer
                if (Physics.SphereCast(mouseRay, 0.4f, out mousetHit, 100f, 1 << BackgroundSetting.BodyPartLayer))
                {
                    if (mousetHit.transform != pullingLimb)
                    {
                        if (pullingLimbScript != null)
                        {
                            pullingLimbScript.Highlight(false);
                        }
                        pullingLimb = mousetHit.transform;
                        pullingLimbScript = pullingLimb.GetComponent<BodyPartCtrl>();
                        pullingLimbScript.Highlight(true);

                    }
                    //Debug.DrawRay(mousetHit.point, Vector3.up*100f, Color.black);

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!pullingLimbScript.separated && !pullingLimbScript.GetOwnerScript().loaded)
                        {
                            mouseScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            mouseScreenPos.z = pullingLimb.position.z;
                            pullingLimbScript.BeingPull(mouseScreenPos, true);
                            limbPulling = true;
                        }
                    }
                }
                else
                {
                    if (pullingLimbScript != null)
                    {
                        pullingLimbScript.Highlight(false);
                        pullingLimbScript = null;
                        pullingLimb = null;
                    }
                }
            }
        }
    }

    //fixedDeltaTime = 0.0167 (60fps)
    protected void SlowMotionCheck()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!timeSlowed)
            {
                timeSlowed = true;
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.0167f * Time.timeScale;
                uiCtrl.VigOnOff(true);
            }
            slowMotionEndTime = Time.realtimeSinceStartup + slowMotionEndSmooth;
        }
        else
        {
            if (timeSlowed && Time.realtimeSinceStartup > slowMotionEndTime)
            {
                timeSlowed = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.0167f;
                uiCtrl.VigOnOff(false);
            }
        }
    }

    protected override void CheckMoveRequest()
    {
        if (!limbPulling)
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            if (inputHorizontal != 0)
            {
                needToMove = true;
                targetDir.x = Mathf.Sign(inputHorizontal);
                return;
            }
        }
        needToMove = false;
    }

    protected override void CheckRecoverRequest()
    {
        if (!limbPulling && Input.GetMouseButtonDown(1))
        {
            PoseRecover();
            ragdollSwitchAvailableTime = Time.time + ragdollSwitchInterval;
        }
    }

    public override void Death()
    {
        if (timeSlowed)
        {
            timeSlowed = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.0167f;
            uiCtrl.VigOnOff(false);
        }
        base.Death();
    }

    public void OnDestroy()
    {
        if (timeSlowed)
        {
            timeSlowed = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.0167f;
            uiCtrl.VigOnOff(false);
        }
    }
}
