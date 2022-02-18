using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PIPCamera : MonoBehaviour {

    public RawImage uiRawImage;

    private Camera camera;
    private RenderTexture rt;
    private Texture textureMissingCamera;
    private GameObject vrCamera;

    void Awake ()
    {
        rt = new RenderTexture(640, 480, 16, RenderTextureFormat.Default);
        camera = GetComponent<Camera>();
        textureMissingCamera = uiRawImage.texture;
        camera.targetTexture = rt;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        SetVRCamera();
    }

    private void OnSceneChanged(Scene from, Scene to)
    {
        SetVRCamera();
    }

    private void SetVRCamera()
    {
        // TODO: use FistVR/game manager function?
        vrCamera = GameObject.FindWithTag("MainCamera");
        if (vrCamera != null)
            uiRawImage.texture = rt;
        else
            uiRawImage.texture = textureMissingCamera;
    }

    // Update is called once per frame
    void Update ()
    {
		if(vrCamera != null)
        {
            transform.position = vrCamera.transform.position;
            transform.rotation = vrCamera.transform.rotation;
        }
        camera.fieldOfView = MeatKitPlugin.cfgPipFov.Value;
	}
}
