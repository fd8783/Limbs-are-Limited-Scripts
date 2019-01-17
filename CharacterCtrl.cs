using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BodyPart
{
    public Transform transform;
    public Rigidbody rigidbody;
    public Collider collider;
    public BodyPartCtrl bodyPartScript;

    public BodyPart(CharacterCtrl ownerScript, Transform bodyTransform, int partNum)
    {
        transform = bodyTransform;
        rigidbody = transform.GetComponent<Rigidbody>();
        collider = transform.GetComponent<Collider>();
        bodyPartScript = transform.GetComponent<BodyPartCtrl>();
        bodyPartScript.Settle(ownerScript, partNum);
    }
}


public class CharacterCtrl : MonoBehaviour {
    
    //player movement control
    protected bool inControl = true, needToMove = false, readyToMove = false, isGround = false, ragdollMode = false;
    [SerializeField]
    protected Vector3 curPos, curForward, targetDir = Vector3.right, footOffset = Vector3.zero;
    protected Quaternion curRot;
    private float tolerateMoveAngle = 1f, maxRotateAngle = 10f, nextAbleMoveTime, minMoveInterval = 0.5f;
    private Rigidbody mainRB;
    private CapsuleCollider mainCol;
    protected float ragdollSwitchAvailableTime = 0f, ragdollSwitchInterval = 1f;

    //body part
    [Tooltip("head, upperBody, lowerBody, leftArm, rightArm, leftLeg, rightLeg")]
    public Transform[] bodyPartForInit = new Transform[7];
    public BodyPart[] bodyPart;  //init in SettleBodyPart()
    protected List<int> connectedBodyPart = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6 });
    protected int bodyPartCount;

    //fly control
    public bool loaded = false, poseFixed = true;
    public GameObject mouth;

    //limbs (Body Part)
    protected bool limbPulling = false;

    // Use this for initialization
    protected virtual void Awake () {
        mainRB = GetComponent<Rigidbody>();
        mainCol = GetComponent<CapsuleCollider>();
        SettleBodyPart();
        //ragdollScript = GetComponent<RagdollCtrl>();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        UpdateCurInfo();

        CheckLimbPulling();

        if (inControl)
        {
            CheckRagdollSwitch();
            CheckMoveRequest();
            CheckIsGround();    //easy check for standing pose
            if (needToMove && isGround)
            {
                if (CheckReadyToMove())
                {
                    MoveToTarget();
                }
                else
                {
                    RotateToTargetDir();
                }
            }
        }
        else
        {
            if (!loaded)
            {
                poseFixed = CheckPoseFixed();   //pose fix should mean the guy land
                if (poseFixed)
                {
                    CheckRecoverRequest();
                }
            }
        }
    }

    void SettleBodyPart()
    {
        bodyPartCount = bodyPartForInit.Length;
        bodyPart = new BodyPart[bodyPartCount];
        for (int i= 0; i < bodyPart.Length; i++)
        {
            bodyPart[i] = new BodyPart(this, bodyPartForInit[i], i);
        }
    }

    public void BodyPartSeparated(int partNum, float strength)
    {
        connectedBodyPart.Remove(partNum);
        ScreenShake.ShakeFrame(0.15f, 3);
        bodyPart[partNum].rigidbody.AddForce(-bodyPart[partNum].transform.up * Mathf.Min(strength, 6f) * 1000f / Time.timeScale);
        if (partNum == 2)
        {
            for (int i =5; i <= 6; i++)
            {
                if (connectedBodyPart.Contains(i))
                {
                    connectedBodyPart.Remove(i);
                    bodyPart[i].bodyPartScript.UpperLevelBeingSeparated();
                }
            }
        }
        else if (partNum == 1)
        {
            for (int i = 2; i <= 6; i++)
            {
                if (connectedBodyPart.Contains(i))
                {
                    connectedBodyPart.Remove(i);
                    bodyPart[i].bodyPartScript.UpperLevelBeingSeparated();
                }
            }
        }
    }

    void UpdateCurInfo()
    {
        curPos = transform.position;
        curForward = transform.forward;
        curRot = transform.rotation;
    }

    #region Limbs Handle
    protected virtual void CheckLimbPulling()
    {

    }
    #endregion

    #region Movement
    protected virtual void CheckMoveRequest()
    {

    }

    bool CheckReadyToMove()
    {
        if (Vector3.Angle(curForward, targetDir) < tolerateMoveAngle)
        {
            if (Time.time > nextAbleMoveTime)
            {
                return true;
            }
        }
        return false;
    }

    void RotateToTargetDir()
    {
        transform.rotation = Quaternion.RotateTowards(curRot, Quaternion.LookRotation(targetDir), maxRotateAngle);
    }

    void CheckIsGround()
    {
        Debug.DrawRay(curPos + footOffset, -transform.up*0.1f, Color.blue);
        isGround = Physics.Raycast(curPos + footOffset, -transform.up, 0.15f); //, 1<<BackgroundSetting.GroundLayer
    }

    Vector3 tempMoveDir;
    void MoveToTarget()
    {
        //currently using addforce to jump 

        tempMoveDir = targetDir;
        tempMoveDir.y = Mathf.Abs(tempMoveDir.x)*0.8f;
        mainRB.AddForce(tempMoveDir * 300f / Time.timeScale);

        nextAbleMoveTime = Time.time + minMoveInterval;
    }
    #endregion

    #region LostControl 
    protected virtual void CheckRecoverRequest()
    {

    }

    void CheckRagdollSwitch()
    {
        if (Input.GetMouseButtonDown(1) && Time.time > ragdollSwitchAvailableTime)
        {
            SetInControl(false);
            SetRagdollAble(true);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void SetInControl(bool isInControl)
    {
        inControl = isInControl;
        mainCol.enabled = isInControl;
        mainRB.isKinematic = !isInControl;
    }

    public void SetRagdollAble(bool isAble)
    {
        ragdollMode = isAble;
        foreach ( int i in connectedBodyPart)
        {
            bodyPart[i].rigidbody.isKinematic = !isAble;
            bodyPart[i].collider.isTrigger = !isAble;
        }
    }

    public void LoadedOnCannon()
    {
        loaded = true;
        //hands up
        if (connectedBodyPart.Contains(3))
            bodyPart[3].transform.localRotation = Quaternion.Euler(0, 0, 180);
        if (connectedBodyPart.Contains(4))
            bodyPart[4].transform.localRotation = Quaternion.Euler(0, 0, 180);
    }

    public void OnFly(float strength)
    {
        loaded = false;
        mouth.SetActive(true);
        SetRagdollAble(true);
        //need more checking
        //Debug.Log(strength / Time.timeScale + "   " + strength + "    " + Time.timeScale);
        bodyPart[0].rigidbody.AddForce(transform.up * strength / Time.timeScale);
    }

    bool CheckPoseFixed()
    {
        foreach (int i in connectedBodyPart)
        {
            if (!bodyPart[i].bodyPartScript.poseFixed)
            {
                return false;
            }
        }
        return true;
    }

    Vector3 bodyPartMeanPos, headPrePos;
    protected void PoseRecover()
    {
        SetInControl(true);
        SetRagdollAble(false);
        loaded = false;
        //mouth.SetActive(false);

        bodyPartMeanPos = Vector3.zero;
        headPrePos = bodyPart[0].transform.position;
        foreach (int i in connectedBodyPart)
        {
            bodyPart[i].bodyPartScript.StartRecover();
            bodyPartMeanPos += bodyPart[i].transform.position;
        }
        bodyPartMeanPos /= connectedBodyPart.Count;
        bodyPartMeanPos.z = 0;
        transform.position = bodyPartMeanPos;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        bodyPart[0].transform.position = headPrePos;

        //resize Collider
        if (!connectedBodyPart.Contains(1))
        {
            mainCol.center = new Vector3(0f, 2.07f, 0f);
            mainCol.radius = 0.2f;
            footOffset.y += mainCol.height - 0.41f;
            mainCol.height = 0.41f;
        }
        else if (!connectedBodyPart.Contains(2))
        {
            mainCol.center = new Vector3(0f, 1.73f, 0f);
            footOffset.y += mainCol.height - 1.08f;
            mainCol.height = 1.08f;
        }
        else if (!(connectedBodyPart.Contains(5) || connectedBodyPart.Contains(6)))
        {
            mainCol.center = new Vector3(0f, 1.58f, 0f);
            footOffset.y += mainCol.height - 1.38f;
            mainCol.height = 1.38f;
        }

        mainRB.AddForce(Vector3.up * 400f / Time.timeScale);
    }

    public virtual void Death()
    {
        transform.SetParent(null);
        SetRagdollAble(true);
        SetInControl(false);
        for (int i = 1; i <= 6; i++)
        {
            if (connectedBodyPart.Contains(i))
            {
                connectedBodyPart.Remove(i);
                bodyPart[i].bodyPartScript.UpperLevelBeingSeparated();
            }
        }
        this.enabled = false;
    }
    #endregion
}
