#if H3VR_IMPORTED
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using FistVR;
using Sodalite.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool uiIsVisible = false;

    // --- CONFIGURATION --- //
    public static ConfigEntry<bool> cfgMousePitchFlip;
    public static ConfigEntry<bool> cfgMouseYawFlip;
    public static ConfigEntry<float> cfgMouseSpeed;

    public static AssetBundle bundle;
    private GameObject mainUI;

    private void Awake()
    {
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "msk_desktopfreecam"));

        // CONFIGURATION
        cfgMousePitchFlip = Config.Bind(
            "Controls",
            "Mouse Pitch Flip",
            false,
            "Flip the mouse look direction on the pitch axis.");
        cfgMouseYawFlip = Config.Bind(
            "Controls",
            "Mouse Yaw Flip",
            false,
            "Flip the mouse look direction on the yaw axis.");
        cfgMouseSpeed = Config.Bind(
            "Controls",
            "Mouse Sensitivity",
            2f,
            "The speed which the camera turns relative to mouse movement.");

        Instantiate(bundle.LoadAsset<GameObject>("IntroText"));

        LoadAssets(); // DO NOT REMOVE
    }

    private void Start()
    {
        mainUI = Instantiate(bundle.LoadAsset<GameObject>("MainUI"));
        DontDestroyOnLoad(mainUI);
        SetUIVisibility(uiIsVisible);
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
        if(Input.GetKeyDown(KeyCode.F3))
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