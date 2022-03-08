using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;

using FistVR;

namespace DesktopFreecam
{
    public class Settings : MonoBehaviour
    {
        // --- CONFIGURATION --- //
        // Mouse
        public static ConfigEntry<bool> cfgMousePitchFlip;
        public static ConfigEntry<bool> cfgMouseYawFlip;
        public static ConfigEntry<float> cfgMouseSensitivity;
        public static ConfigEntry<ScrollWheelMode> cfgScrollMode;
        // Camera
        public static ConfigEntry<float> cfgCameraFov;
        public static ConfigEntry<float> cfgCameraFlySpeed;
        public static ConfigEntry<float> cfgCameraFlyFastMult;
        public static ConfigEntry<float> cfgCameraFlySlowMult;
        // Picture in picture
        public static ConfigEntry<float> cfgPipOpacity;
        public static ConfigEntry<bool> cfgPipUnlocked;
        public static ConfigEntry<int> cfgPipResX;
        public static ConfigEntry<int> cfgPipResY;
        // Keyboard
        public static Dictionary<KBControls, ConfigEntry<UnityEngine.KeyCode>> cfgKeyboard;

        // --- RUNTIME STATES ---//
        // these don't get saved anywhere.
        public static State<float> specFov = new State<float>(75);
        public static State<bool> specWindowDraggable = new State<bool>(false);
        public static State<bool> freecamVisibleToPlayer = new State<bool>(false);
        public static State<bool> pipEnabled = new State<bool>(false);
        public static State<SpectatorMode> mainViewMode = new State<SpectatorMode>(SpectatorMode.FVRSpectator);

        private void Awake()
        {
            SyncSpecFOV(null, null);
            specFov.ValueChanged += SyncSpecFOV;
        }

        private void SyncSpecFOV(object sender, System.EventArgs e)
        {
            Util.FVRSetSpectatorFOV(specFov.Value);
        }
    }

    // TODO: patch SpectatorPanel to run specFov.SetValue
    [HarmonyPatch(typeof(SpectatorPanel))]
    [HarmonyPatch("BTN_SetFOV", MethodType.Normal)]
    class SpectatorPanelFOVPatch
    {
        private static void Postfix(SpectatorPanel __instance)
        {
            Settings.specFov.Value = GM.Options.ControlOptions.CamFOV;
        }
    }
}