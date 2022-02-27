#if H3VR_IMPORTED
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
    public static ConfigEntry<float> cfgPipFov;
    public static ConfigEntry<bool> cfgPipEditWindow;
    public static ConfigEntry<float> cfgPipWindowOpacity;
    public static ConfigEntry<int> cfgPipResX;
    public static ConfigEntry<int> cfgPipResY;
    // Keyboard
    public static Dictionary<KBControls, ConfigEntry<KeyCode>> cfgKeyboard;
    //public static ConfigEntry<KeyCode> cfgKbForward;
    //public static ConfigEntry<KeyCode> cfgKbBackward;
    //public static ConfigEntry<KeyCode> cfgKbLeft;
    //public static ConfigEntry<KeyCode> cfgKbRight;
    //public static ConfigEntry<KeyCode> cfgKbAscend;
    //public static ConfigEntry<KeyCode> cfgKbDescend;
    //public static ConfigEntry<KeyCode> cfgKbSpeedUp;
    //public static ConfigEntry<KeyCode> cfgKbSlowDn;
    //public static ConfigEntry<KeyCode> cfgKbToggleControls;
    //public static ConfigEntry<KeyCode> cfgKbToggleUI;
    // Test: pair of ints config

    private void Awake()
    {
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "msk_desktopfreecam"));

        // --- CONFIGURATION ---
        // [Mouse]
        cfgMousePitchFlip = Config.Bind(
            "Mouse",
            "Mouse Pitch Flip",
            false,
            "Flip the mouse look direction on the pitch axis.");
        cfgMouseYawFlip = Config.Bind(
            "Mouse",
            "Mouse Yaw Flip",
            false,
            "Flip the mouse look direction on the yaw axis.");
        cfgMouseSensitivity = Config.Bind(
            "Mouse",
            "Mouse Sensitivity",
            2f,
            "The speed which the camera turns relative to mouse movement.");
        cfgScrollMode = Config.Bind(
            "Mouse",
            "Scroll wheel mode",
            ScrollWheelMode.MoveSpeed,
            "What the scroll wheel does while in control.");
        // [Camera]
        cfgCameraFov = Config.Bind(
            "Camera",
            "Field of view",
            75f,
            "Field of view on the free-flying camera.");
        cfgCameraFlySpeed = Config.Bind(
            "Camera",
            "Fly speed",
            4f,
            "Base movement speed in meters per second..");
        cfgCameraFlyFastMult = Config.Bind(
            "Camera",
            "Fast flying multplier",
            2.2f,
            "Multiplier for the camera speed when the \"Speed up\" button is held.");
        cfgCameraFlySlowMult = Config.Bind(
            "Camera",
            "Slow flying multplier",
            .45f,
            "Multiplier for the camera speed when the \"Slow down\" button is held.");
        // [Picture in picture]
        cfgPipFov = Config.Bind(
            "Picture in picture",
            "Field of view",
            75f,
            "Field of view on the PIP camera.");
        cfgPipEditWindow = Config.Bind(
            "Picture in picture",
            "Unlock view window",
            false,
            "Allow view window to be moved and rescaled.");
        cfgPipWindowOpacity = Config.Bind(
            "Picture in picture",
            "Window opacity",
            1f,
            "Set opacity of the PIP window.");
        cfgPipResX = Config.Bind(
            "Picture in picture",
            "Window resolution (x)",
            640);
        cfgPipResY = Config.Bind(
            "Picture in picture",
            "Window resolution (Y)",
            400);
        // [Keyboard] (oh boy)
        cfgKeyboard = new Dictionary<KBControls, ConfigEntry<KeyCode>>
            (System.Enum.GetNames(typeof(KBControls)).Length);
        cfgKeyboard[KBControls.MoveForward] = Config.Bind(
            "Keyboard",
            "Move forward",
            KeyCode.W);
        cfgKeyboard[KBControls.MoveBackward] = Config.Bind(
            "Keyboard",
            "Move backward",
            KeyCode.S);
        cfgKeyboard[KBControls.MoveLeft] = Config.Bind(
            "Keyboard",
            "Move left",
            KeyCode.A);
        cfgKeyboard[KBControls.MoveRight] = Config.Bind(
            "Keyboard",
            "Move right",
            KeyCode.D);
        cfgKeyboard[KBControls.Ascend] = Config.Bind(
            "Keyboard",
            "Ascend",
            KeyCode.Space);
        cfgKeyboard[KBControls.Descend] = Config.Bind(
            "Keyboard",
            "Descend",
            KeyCode.LeftControl);
        cfgKeyboard[KBControls.SpeedUp] = Config.Bind(
            "Keyboard",
            "Speed up",
            KeyCode.LeftShift);
        cfgKeyboard[KBControls.SlowDown] = Config.Bind(
            "Keyboard",
            "Slow down",
            KeyCode.LeftAlt);
        cfgKeyboard[KBControls.TeleportToPlayer] = Config.Bind(
            "Keyboard",
            "Teleport to player",
            KeyCode.R);
        cfgKeyboard[KBControls.ToggleControls] = Config.Bind(
            "Keyboard",
            "Toggle controls",
            KeyCode.Tab);
        cfgKeyboard[KBControls.ToggleUI] = Config.Bind(
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
        if (MainUI.freecam != null)
        {
            MainUI.freecam.GetComponentInChildren<CanvasGroup>().alpha = state ? 1 : 0;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(cfgKeyboard[KBControls.ToggleUI].Value))
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