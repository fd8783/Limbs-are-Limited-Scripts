using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartCtrl : MonoBehaviour {

    //init
    public ParticleSystem[] bloodParticles;
    private Rigidbody bodyRB;
    private Quaternion startLocRot;
    private Vector3 startLocPos;
    private Transform model;
    private MeshRenderer modelMesh;
    private Material modelDefaultMat;
    private CharacterCtrl ownerScript;
    private int bodyPartNum = -1;
    private AudioSource pullSound;

    //checking
    public bool poseFixed = false, separated = false;
    private Vector3 curPos;
    private Quaternion curRot;
    private float posFixConfirmTime = 1f, posFixTimeCounter = 0;

    //recover to origin pose
    private bool poseRecovering = false;
    private float recoverTime = 0.5f, recoverTimeCounter = 0;

    //pulling
    private bool beingPull = false, highlighted = false;
    private HingeJoint bodyJoint;
    [SerializeField]
    private float maxRotateAngle = 10f;
    private float startZRot, minZRot, maxZRot;
    private float pullingDis = 0f, breakDis = 4f, modelStartScaY;
    private Vector3 curPullingPos;
    private Vector3 modelSca;

    public void Settle(CharacterCtrl ownerScript, int partNum)
    {
        this.ownerScript = ownerScript;
        bodyPartNum = partNum;
    }

	// Use this for initialization
	void Awake () {
        bodyRB = GetComponent<Rigidbody>();
        curPos = transform.position;
        curRot = transform.rotation;
        startLocRot = transform.localRotation;
        startLocPos = transform.localPosition;
        model = transform.Find("Model");
        modelSca = model.localScale;
        modelStartScaY = modelSca.y;
        modelMesh = model.GetComponent<MeshRenderer>();
        modelDefaultMat = modelMesh.material;
        pullSound = GetComponent<AudioSource>();

        startZRot = startLocRot.z;
        minZRot = startZRot;
        maxZRot = startZRot;
        bodyJoint = GetComponent<HingeJoint>();
        if (bodyJoint != null)
        {
            minZRot += bodyJoint.limits.min+10;
            maxZRot += bodyJoint.limits.max-10;
        }
    }
	
	// Update is called once per frame
	void Update () {
        CheckPosFixed();
        if (beingPull)
        {

        }
        else if (poseRecovering)
        {
            PoseRecover();
        }
    }

    void CheckPosFixed()
    {
        if (Quaternion.Angle(transform.rotation, curRot) <1f && Vector3.SqrMagnitude(transform.position - curPos) < 0.001f)  //magnitude < 0.01f
        {
            if (!poseFixed && Time.time > posFixTimeCounter)
            {
                poseFixed = true;
            }
        }
        else
        {
            poseFixed = false;
            posFixTimeCounter = Time.time + posFixConfirmTime;
        }
        curPos = transform.position;
        curRot = transform.rotation;
    }

    public void StartRecover()
    {
        recoverTimeCounter = 0;
        poseRecovering = true;
    }

    void PoseRecover()
    {
        recoverTimeCounter += Time.deltaTime / recoverTime;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, startLocRot, recoverTimeCounter);
        transform.localPosition = Vector3.Slerp(transform.localPosition, startLocPos, recoverTimeCounter);

        if (recoverTimeCounter >= 1f)
        {
            poseRecovering = false;
        }
    }

    public void BeingPull(Vector3 startPullPos, bool isBeingPull)
    {
        beingPull = isBeingPull;
        this.curPullingPos = startPullPos;
        if (!beingPull)
        {
            modelSca.y = modelStartScaY;
            model.localScale = modelSca;
        }
    }

    public void BeingPull(float strength)
    {
        if (strength > breakDis)
        {
            //Debug.Log(pullingDis);
            modelSca.y = modelStartScaY;
            model.localScale = modelSca;
            Separated(strength);
        }
    }

    Vector3 tempEulerAngle;
    Quaternion targetRot;
    //public float c;
    public void Pulling(Vector3 pullVector)
    {
        if (separated) return;

        //transform.position = pullTargetPos;
        curPullingPos += pullVector;
        curPullingPos.z = transform.position.z;

        //targetRot = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, (transform.position - curPullingPos).normalized), maxRotateAngle*5);
        //Vector3 torqueF = Vector3.Cross(transform.forward, targetRot * Vector3.forward);
        //Vector3 torqueR = Vector3.Cross(transform.right, targetRot * Vector3.right);
        //Vector3 torqueU = Vector3.Cross(transform.up, targetRot * Vector3.up);
        //Vector3 torque = torqueF * Vector3.Angle(transform.forward, targetRot * Vector3.forward) / 90f
        //    + torqueR * Vector3.Angle(transform.right, targetRot * Vector3.right) / 90f
        //    + torqueU * Vector3.Angle(transform.up, targetRot * Vector3.up) / 90f;
        //bodyRB.AddTorque(torque * 50000f / Time.timeScale, ForceMode.VelocityChange); 
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, (transform.position - curPullingPos).normalized), maxRotateAngle);

        //handleZRot
        tempEulerAngle = transform.localEulerAngles;
        if (tempEulerAngle.z >= 180)
        {
            tempEulerAngle.z -= 360;
        }
        //c = tempEulerAngle.z;
        tempEulerAngle.z = Mathf.Clamp(tempEulerAngle.z, minZRot, maxZRot);
        transform.localEulerAngles = tempEulerAngle;

        pullingDis = Vector3.Distance(curPullingPos, transform.position);
        modelSca.y = modelStartScaY * Mathf.Clamp(Mathf.Log(pullingDis, 2), 1, 1.8f);
        model.localScale = modelSca;

        BeingPull(pullingDis);
    }

    public void Separated(float strength)
    {
        bodyJoint.connectedBody.AddForce(transform.up * Mathf.Min(strength, 6f) * 2000f / Time.timeScale);
        pullSound.pitch = Random.Range(0.8f, 1.1f);
        pullSound.volume = Random.Range(0.5f, 0.8f);
        pullSound.Play();

        Destroy(bodyJoint);
        transform.SetParent(null);
        gameObject.layer = BackgroundSetting.DullLayer;
        separated = true;
        for (int i = 0; i < bloodParticles.Length; i++)
        {
            bloodParticles[i].Play();
            Destroy(bloodParticles[i].gameObject, 30f);
        }
        ownerScript.BodyPartSeparated(bodyPartNum, strength);

    }

    public void UpperLevelBeingSeparated()
    {
        //keep ragdoll on this part
        gameObject.layer = BackgroundSetting.DullLayer;
        separated = true;
    }

    public void Highlight(bool isHighlight)
    {
        highlighted = isHighlight;
        modelMesh.material = isHighlight ? BackgroundSetting.highlightMat : modelDefaultMat;
    }

    public void Blooded()
    {
        modelMesh.material = BackgroundSetting.bloodRedMat;
    }

    public CharacterCtrl GetOwnerScript()
    {
        return ownerScript;
    }

}
