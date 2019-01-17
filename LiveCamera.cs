using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveCamera : MonoBehaviour {

    private Camera liveCamera;
    private int downResFactor = 1;

    private string liveCamTexName = "_liveCamTex";

    private void OnEnable()
    {
        liveCamera = transform.GetComponent<Camera>();
        GenerateRT();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void GenerateRT()
    {
        liveCamera.targetTexture = new RenderTexture(liveCamera.pixelWidth >> downResFactor, liveCamera.pixelHeight >> downResFactor, 16);
        liveCamera.targetTexture.filterMode = FilterMode.Bilinear;

        Shader.SetGlobalTexture(liveCamTexName, liveCamera.targetTexture);
    }
}
