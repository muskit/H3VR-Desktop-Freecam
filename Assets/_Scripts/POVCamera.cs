using System;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using FistVR;

namespace DesktopFreecam
{
    public class POVCamera : MonoBehaviour
    {

        public RawImage uiRawImage;

        private RenderTexture rt;
        //private GameObject gObjVRCamera;
        private Camera specCamera;
        private Texture textureMissingCamera;
        private UIDraggable windowDraggable;

        #region INITIALIZATION
        public void SetPIP(bool pipEnabled)
        {
            var specPanel = FindObjectOfType<SpectatorPanel>();
            if (pipEnabled)
            {
                //povCamera = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("POVCamera"));
                //DontDestroyOnLoad(povCamera);
                if (specPanel != null)
                {
                    specPanel.BTN_SetCamMode((int)ControlOptions.DesktopCameraMode.HDSpectator);
                }
                else
                    GM.Options.ControlOptions.CamMode = ControlOptions.DesktopCameraMode.HDSpectator;
            }
            else
            {
                //Destroy(povCamera);
                if (specPanel != null)
                {
                    specPanel.BTN_SetCamMode((int)ControlOptions.DesktopCameraMode.HDSpectator);
                }
                else
                    GM.Options.ControlOptions.CamMode = ControlOptions.DesktopCameraMode.HDSpectator;
            }
        }
        void Awake()
        {
            //specCamera = GetComponent<Camera>();
            textureMissingCamera = uiRawImage.texture;
            windowDraggable = uiRawImage.GetComponent<UIDraggable>();
            MeatKitPlugin.cfgPipResX.SettingChanged += OnResXChange;
            MeatKitPlugin.cfgPipResY.SettingChanged += OnResYChange;

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void Start()
        {
            InitializePIP();
            var specPanel = FindObjectOfType<SpectatorPanel>();
            if (specPanel != null)
            {
                specPanel.BTN_SetCamMode((int)ControlOptions.DesktopCameraMode.HDSpectator);
            }
            else
                GM.Options.ControlOptions.CamMode = ControlOptions.DesktopCameraMode.HDSpectator;
        }

        private void OnSceneChanged(Scene from, Scene to)
        {
            InitializePIP();
        }

        private void InitializePIP()
        {
            var specPanel = FindObjectOfType<SpectatorPanel>();
            if (specPanel != null)
            {
                specPanel.BTN_SetCamMode((int)ControlOptions.DesktopCameraMode.HDSpectator);
            }
            else
                GM.Options.ControlOptions.CamMode = ControlOptions.DesktopCameraMode.HDSpectator;
        }
        #endregion

        private void OnResXChange(object sender, EventArgs e)
        {
            SetResolution(width: MeatKitPlugin.cfgPipResX.Value);
        }

        private void OnResYChange(object sender, EventArgs e)
        {
            SetResolution(height: MeatKitPlugin.cfgPipResY.Value);
        }

        private void SetResolution(int width = -1, int height = -1)
        {
            if (rt != null)
                rt.Release();
            var a = rt;
            Destroy(a);

            rt = new RenderTexture((width > 0 ? width : MeatKitPlugin.cfgPipResX.Value),
                (height > 0 ? height : MeatKitPlugin.cfgPipResY.Value), 16, RenderTextureFormat.Default);
            specCamera.targetTexture = rt;
            uiRawImage.texture = rt; // FIXME: causing exception

            if (specCamera != null)
                uiRawImage.texture = rt;
            else
                uiRawImage.texture = textureMissingCamera;

            uiRawImage.rectTransform.sizeDelta = new Vector2(uiRawImage.texture.width, uiRawImage.texture.height);
        }

        private void SetSpecCamera()
        {
            // TODO: use FistVR/game manager function?
            var foundObj = GameObject.Find("[SpectatorCamera](Clone)");
            if (foundObj != null)
            {
                specCamera = foundObj.GetComponent<Camera>();
            }
        }

        // TODO: scaling
        private void SetWindowEditable(bool val)
        {
            windowDraggable.enabled = val;
        }

        private void Update()
        {
            // INITIALIZATION
            if (specCamera == null && GM.Options.ControlOptions.CamMode != ControlOptions.DesktopCameraMode.Default)
            {
                SetSpecCamera();
                SetResolution(MeatKitPlugin.cfgPipResX.Value, MeatKitPlugin.cfgPipResY.Value);
            }

            if (specCamera != null)
                specCamera.fieldOfView = MeatKitPlugin.cfgPipFov.Value;

            uiRawImage.canvasRenderer.SetAlpha(MeatKitPlugin.cfgPipWindowOpacity.Value);
            SetWindowEditable(MeatKitPlugin.cfgPipEditWindow.Value);
        }

        private void OnDestroy()
        {
            var specPanel = FindObjectOfType<SpectatorPanel>();
            if (specPanel != null)
            {
                specPanel.BTN_SetCamMode((int)ControlOptions.DesktopCameraMode.Default);
            }
            else
                GM.Options.ControlOptions.CamMode = ControlOptions.DesktopCameraMode.Default;
            rt.Release();
        }
    }
}