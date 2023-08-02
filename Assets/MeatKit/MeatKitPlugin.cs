#if H3VR_IMPORTED
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

using UnityEngine;
using UnityEngine.SceneManagement;

using FistVR;
using DesktopFreecam;

/*
 * SUPER LARGE WARNING ABOUT THIS CLASS
 * This is the default and fallback class that MeatKit uses as a template to generate a BepInEx plugin
 * when building your mod. DO NOT MODIFY THIS FILE AT ALL, IN ANY WAY.
 *
 * If you want to add custom behavior to your mod, you should make a copy of this class, and put it inside
 * the main namespace of your mod (that namespace can be found by opening the 'Allowed Namespaces' list on your build
 * profile). MeatKit will then detect and use that class instead of this one, for that one specific profile.
 *
 * HOWEVER, YOU MUST KEEP ALL OF THE STUFF FROM THIS TEMPLATE, otherwise MeatKit may fail to correctly build
 * your plugin, or your mod may fail to correctly load.
 */

// DO NOT REMOVE OR CHANGE ANY OF THESE ATTRIBUTES
[BepInPlugin("MeatKit", "MeatKit Plugin", "1.0.0")]
[BepInProcess("h3vr.exe")]

// DO NOT CHANGE THE NAME OF THIS CLASS OR THE BASE CLASS. If you're making a custom plugin, make sure it extends BaseUnityPlugin.
public class MeatKitPlugin : BaseUnityPlugin
{
    // DO NOT CHANGE OR REMOVE THIS FIELD.
#pragma warning disable 414
    private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    internal new static ManualLogSource Logger;
#pragma warning restore 414

    public static AssetBundle bundle;
    public static GameObject mainUI;
    public static FVRSceneSettings sceneSettings;

    private bool uiIsVisible = false;

    public MeatKitPlugin() : base()
    {
        new Harmony("muskit.DesktopFreecam").PatchAll();
    }

    // You are free to edit this method, however please ensure LoadAssets is still called somewhere inside it.
    private void Awake()
    {
        /// BEGIN MEATKIT REQUIRED CODE ///
        // This lets you use your BepInEx-provided logger from other scripts in your project
        Logger = base.Logger;
        
        // You may place code before/after this, but do not remove this call to LoadAssets
        LoadAssets();
        /// END MEATKIT REQUIRED CODE ///
        
        // load AssetBundle
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "muskit_desktopfreecam"));

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
    }

    private void Start()
    {
		Logger.LogInfo("A");
		mainUI = Instantiate(bundle.LoadAsset<GameObject>("MainUI"));
        Logger.LogInfo("B");
        DontDestroyOnLoad(mainUI);
        Logger.LogInfo("C");
        SetUIVisibility(uiIsVisible);

        Logger.LogInfo("D");
        var transIntro = Instantiate(bundle.LoadAsset<GameObject>("IntroText")).transform;
        Logger.LogInfo("E");
        transIntro.SetParent(GameObject.FindGameObjectWithTag("MainCamera").transform);
        Logger.LogInfo("F");
        transIntro.position = new Vector3(999, 999, 999);
        Logger.LogInfo("G");
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
        // Code to load your build items will be generated at build-time and inserted here
    }
}
#endif
