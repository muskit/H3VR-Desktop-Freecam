using System;
using UnityEngine;
using BepInEx.Configuration;

using FistVR;

namespace DesktopFreecam
{
    public enum SpectatorMode { FVRSpectator, Freecam }

    [RequireComponent(typeof(Settings))]
    public class SpectatorManager : MonoBehaviour
    {
        private Settings settings;
        private SlidePanel uiSlideMenu;

        private PIPWindow pipWindow;

        private Camera specCamera; // for setting up render texture
        private Freecam freecam;

        private void Awake()
        {
            settings = GetComponent<Settings>();
            uiSlideMenu = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("SlidePanel"), transform).GetComponent<SlidePanel>();
            pipWindow = GetComponentInChildren<PIPWindow>();
            freecam = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Freecam"), transform).GetComponent<Freecam>();

            Settings.mainViewMode.ValueChanged += OnMainViewChanged;
            Settings.pipEnabled.ValueChanged += OnCfgPipEnabledChanged;
            Settings.cfgPipUnlocked.SettingChanged += OnCfgPipUnlockedChanged;
        }


        private void Start()
        {
            UpdateView();
        }

        // trig by Settings.mainViewMode.ValueChanged
        private void OnMainViewChanged(object s, System.EventArgs e)
        {
            UpdateView();
        }

        // trig by Settings.pipEnabled.ValueChanged
        private void OnCfgPipEnabledChanged(object sender, EventArgs e)
        {
            pipWindow.gameObject.SetActive(((State<bool>)sender).Value);
            UpdateView();
        }

        // trig by Settings.cfgPipUnlocked.SettingChanged
        private void OnCfgPipUnlockedChanged(object sender, EventArgs e)
        {
            pipWindow.SetWindowDraggable(((ConfigEntry<bool>)sender).Value);
        }

        private void GetSpecCamera()
        {
            // TODO: use FistVR/game manager function?
            if (Util.FVRGetSpectatorMode() == ControlOptions.DesktopCameraMode.Default)
            {
                specCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            else
            {
                var foundObj = GameObject.Find("[SpectatorCamera](Clone)");
                if (foundObj != null)
                {
                    specCamera = foundObj.GetComponent<Camera>();
                    specCamera.cullingMask = -268435457;
                }
            }
        }

        private void UpdateView()
        {
            GetSpecCamera();
            
            pipWindow.gameObject.SetActive(Settings.pipEnabled.Value);
            switch (Settings.mainViewMode.Value) // TODO: work out logic (CRITICAL)
            {
                case SpectatorMode.FVRSpectator:
                    specCamera.targetTexture.Release();
                    if (Settings.pipEnabled.Value)
                    {
                        freecam.gameObject.SetActive(true);
                        pipWindow.SetCamera(freecam.camera);
                    }
                    else
                    {
                        freecam.gameObject.SetActive(false);
                    }
                    break;
                case SpectatorMode.Freecam:
                    freecam.gameObject.SetActive(true);
                    if (Settings.pipEnabled.Value)
                    {
                        pipWindow.SetCamera(specCamera);
                    }
                    else
                    {
                        Util.FVRSetSpectatorMode(ControlOptions.DesktopCameraMode.Default);
                    }
                    break;
            }
        }
        
        void Update()
        {

        }

        private void OnDestroy()
        {
            Settings.mainViewMode.ValueChanged -= OnMainViewChanged;
            Settings.cfgPipUnlocked.SettingChanged -= OnCfgPipUnlockedChanged;
            Settings.pipEnabled.ValueChanged -= OnCfgPipEnabledChanged;

            Util.FVRSetSpectatorMode(ControlOptions.DesktopCameraMode.Default);
        }
    }
}