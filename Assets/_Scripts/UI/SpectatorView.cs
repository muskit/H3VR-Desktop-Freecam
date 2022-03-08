//using System;
//using BepInEx.Configuration;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//using FistVR;

//namespace DesktopFreecam
//{
//    public class SpectatorView : MonoBehaviour
//    {
//        public RenderTexture rt;

//        private Camera specCamera;
//        private Texture textureMissing;
//        private UIDraggable windowDraggable;

//        #region INITIALIZATION
//        void Awake()
//        {
//            //specCamera = GetComponent<Camera>();
//            textureMissing = uiRawImage.texture;
//            windowDraggable = uiRawImage.GetComponent<UIDraggable>();
//            Settings.cfgPipResX.SettingChanged += OnResXChange;
//            Settings.cfgPipResY.SettingChanged += OnResYChange;

//            SceneManager.activeSceneChanged += OnSceneChanged;
//        }

//        private void Start()
//        {
//            InitializePIP();
//        }

//        private void OnSceneChanged(Scene from, Scene to)
//        {
//            InitializePIP();
//        }

//        private void InitializePIP()
//        {
//            Util.FVRSetSpectatorMode(ControlOptions.DesktopCameraMode.HDSpectator);
//        }
//        #endregion

//        private void OnResXChange(object sender, EventArgs e)
//        {
//            SetResolution(width: Settings.cfgPipResX.Value);
//        }

//        private void OnResYChange(object sender, EventArgs e)
//        {
//            SetResolution(height: Settings.cfgPipResY.Value);
//        }

//        private void SetResolution(int width = -1, int height = -1)
//        {
//            if (rt != null)
//                rt.Release();

//            // Destroy() is delayed. Destroy(rt) would destroy our newly
//            // created RT. We set the old one to another variable and
//            // destroy that instead.
//            var a = rt;
//            Destroy(a);

//            rt = new RenderTexture((width > 0 ? width : Settings.cfgPipResX.Value),
//                (height > 0 ? height : Settings.cfgPipResY.Value), 16, RenderTextureFormat.ARGB32);
//            specCamera.targetTexture = rt;
//            uiRawImage.texture = rt;

//            if (specCamera != null)
//                uiRawImage.texture = rt;
//            else
//                uiRawImage.texture = textureMissing;

//            uiRawImage.rectTransform.sizeDelta = new Vector2(uiRawImage.texture.width, uiRawImage.texture.height);
//        }

//        private void GetSpecCamera()
//        {
//            // TODO: use FistVR/game manager function?
//            var foundObj = GameObject.Find("[SpectatorCamera](Clone)");
//            if (foundObj != null)
//            {
//                specCamera = foundObj.GetComponent<Camera>();
//            }
//        }

//        // TODO: scaling
//        private void SetWindowEditable(bool val)
//        {
//            windowDraggable.enabled = val;
//        }

//        private void Update()
//        {
//            // INITIALIZATION
//            if (specCamera == null && GM.Options.ControlOptions.CamMode != ControlOptions.DesktopCameraMode.Default)
//            {
//                GetSpecCamera();
//                SetResolution(Settings.cfgPipResX.Value, Settings.cfgPipResY.Value);
//            }

//            uiRawImage.canvasRenderer.SetAlpha(Settings.cfgPipOpacity.Value);
//            SetWindowEditable(Settings.specEditWindow.Value);
//        }

//        private void OnDestroy()
//        {
//            Util.FVRSetSpectatorMode(ControlOptions.DesktopCameraMode.Default);
//            rt.Release();
//        }
//    }
//}