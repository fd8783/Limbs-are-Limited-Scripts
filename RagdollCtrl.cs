using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCtrl : MonoBehaviour {

    public Rigidbody[] ragdollRBs;
    private Collider[] ragdollCols;
    private int ragdollCount;
    
	// Use this for initialization
	void Awake () {
        ragdollCount = ragdollRBs.Length;
        //RagdollAble(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GetRagdollCols()
    {
        ragdollCols = new Collider[ragdollCount];
        for (int i = 0; i < ragdollCount; i++)
        {
            ragdollCols[i] = ragdollRBs[i].GetComponent<Collider>();
        }
    }

    public void RagdollAble(bool isAble)
    {
        for (int i = 0; i < ragdollCount; i++)
        {
            ragdollRBs[i].isKinematic = !isAble;
            ragdollCols[i].enabled = isAble;
        }
    }
}
