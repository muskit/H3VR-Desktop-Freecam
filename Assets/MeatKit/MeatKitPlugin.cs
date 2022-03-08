#if H3VR_IMPORTED
using HarmonyLib;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

using FistVR;
using DesktopFreecam;

/*
    * SUPER LARGE WARNING ABOUT THIS CLASS
    * This class can be used to add custom behaviour to your generated BepInEx plugin.
    * Please note, however, that all of the things in here already are REQUIRED and CANNOT BE CHANGED.
    * There are LARGE TEXT WARNINGS above such items so you don't forget.
    * You may add to this class so long as you do not modify anything with those notices (lest you want build errors)
    *
    * The class name and BepInPlugin attribute are modified at build-time to reflect your build settings.
    * BepInDependency attributes will automatically be generated if they're required by a build item, otherwise
    * may add it yourself here.
    */

// DO NOT REMOVE OR CHANGE ANY OF THESE ATTRIBUTES
[BepInPlugin("MeatKit", "MeatKit Plugin", "1.0.0")]
[BepInProcess("h3vr.exe")]

// DO NOT CHANGE THE NAME OF THIS CLASS.
public class MeatKitPlugin : BaseUnityPlugin
{
    // DO NOT CHANGE OR REMOVE THIS FIELD.
#pragma warning disable 414
    private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#pragma warning restore 414

    public static AssetBundle bundle;
    public static GameObject mainUI;
    public static FVRSceneSettings sceneSettings;

    private bool uiIsVisible = false;

    public MeatKitPlugin() : base()
    {
        new Harmony("muskit.DesktopFreecam").PatchAll();
    }

    private void Awake()
    {
        Logger.LogInfo("BasePath: " + BasePath);
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "msk_desktopfreecam"));

        // --- CONFIGURATION ---
        // [Mouse]
        Settings.cfgMousePitchFlip = Config.Bind(
            "Mouse",
            "Mouse Pitch Flip",
            false,
            "Flip the mouse look direction on the pitch axis.");
        Settings.cfgMouseYawFlip = Config.Bind(
            "Mouse",
            "Mouse Yaw Flip",
            false,
            "Flip the mouse look direction on the yaw axis.");
        Settings.cfgMouseSensitivity = Config.Bind(
            "Mouse",
            "Mouse Sensitivity",
            2f,
            "The speed which the camera turns relative to mouse movement.");
        Settings.cfgScrollMode = Config.Bind(
            "Mouse",
            "Scroll wheel mode",
            ScrollWheelMode.MoveSpeed,
            "What the scroll wheel does while in control.");
        // [Camera]
        Settings.cfgCameraFov = Config.Bind(
            "Camera",
            "Field of view",
            75f,
            "Field of view on the free-flying camera.");
        Settings.cfgCameraFlySpeed = Config.Bind(
            "Camera",
            "Fly speed",
            4f,
            "Base movement speed in meters per second..");
        Settings.cfgCameraFlyFastMult = Config.Bind(
            "Camera",
            "Fast flying multplier",
            2.2f,
            "Multiplier for the camera speed when the \"Speed up\" button is held.");
        Settings.cfgCameraFlySlowMult = Config.Bind(
            "Camera",
            "Slow flying multplier",
            .45f,
            "Multiplier for the camera speed when the \"Slow down\" button is held.");
        // [Picture in picture]
        Settings.cfgPipOpacity = Config.Bind(
            "Picture in picture",
            "Window opacity",
            1f,
            "Set opacity of the PIP window.");
        Settings.cfgPipResX = Config.Bind(
            "Picture in picture",
            "Window resolution (x)",
            640);
        Settings.cfgPipResY = Config.Bind(
            "Picture in picture",
            "Window resolution (Y)",
            400);
        Settings.cfgPipUnlocked = Config.Bind(
           "Picture in picture",
           "Window is draggable",
           true);
        // [Keyboard] (oh boy)
        Settings.cfgKeyboard = new Dictionary<KBControls, ConfigEntry<KeyCode>>
            (System.Enum.GetNames(typeof(KBControls)).Length);
        Settings.cfgKeyboard[KBControls.MoveForward] = Config.Bind(
            "Keyboard",
            "Move forward",
            KeyCode.W);
        Settings.cfgKeyboard[KBControls.MoveBackward] = Config.Bind(
            "Keyboard",
            "Move backward",
            KeyCode.S);
        Settings.cfgKeyboard[KBControls.MoveLeft] = Config.Bind(
            "Keyboard",
            "Move left",
            KeyCode.A);
        Settings.cfgKeyboard[KBControls.MoveRight] = Config.Bind(
            "Keyboard",
            "Move right",
            KeyCode.D);
        Settings.cfgKeyboard[KBControls.Ascend] = Config.Bind(
            "Keyboard",
            "Ascend",
            KeyCode.Space);
        Settings.cfgKeyboard[KBControls.Descend] = Config.Bind(
            "Keyboard",
            "Descend",
            KeyCode.LeftControl);
        Settings.cfgKeyboard[KBControls.SpeedUp] = Config.Bind(
            "Keyboard",
            "Speed up",
            KeyCode.LeftShift);
        Settings.cfgKeyboard[KBControls.SlowDown] = Config.Bind(
            "Keyboard",
            "Slow down",
            KeyCode.LeftAlt);
        Settings.cfgKeyboard[KBControls.TeleportToPlayer] = Config.Bind(
            "Keyboard",
            "Teleport to player",
            KeyCode.R);
        Settings.cfgKeyboard[KBControls.ToggleControls] = Config.Bind(
            "Keyboard",
            "Toggle controls",
            KeyCode.Tab);
        Settings.cfgKeyboard[KBControls.ToggleUI] = Config.Bind(
            "Keyboard",
            "Toggle UI",
            KeyCode.F3);

        SceneManager.activeSceneChanged += OnSceneChange;

        LoadAssets(); // DO NOT REMOVE
    }

    private void Start()
    {
        mainUI = Instantiate(bundle.LoadAsset<GameObject>("MainUI"));
        DontDestroyOnLoad(mainUI);
        SetUIVisibility(uiIsVisible);

        var transIntro = Instantiate(bundle.LoadAsset<GameObject>("IntroText")).transform;
        transIntro.SetParent(GameObject.FindGameObjectWithTag("MainCamera").transform);
        transIntro.position = new Vector3(999, 999, 999);
    }

    private void OnSceneChange(Scene from, Scene to)
    {
        sceneSettings = FindObjectOfType<FVRSceneSettings>();
    }
    
    private void SetUIVisibility(bool state)
    {
        mainUI.transform.GetChild(0).gameObject.SetActive(state);
    }

    private void Update()
    {
        if(Input.GetKeyDown(Settings.cfgKeyboard[KBControls.ToggleUI].Value))
        {
            uiIsVisible = !uiIsVisible;
            SetUIVisibility(uiIsVisible);
        }
    }

    // DO NOT CHANGE OR REMOVE THIS METHOD. It's contents will be overwritten when building your package.
    private void LoadAssets()
    {
    }
}
#endif