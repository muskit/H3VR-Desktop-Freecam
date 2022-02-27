using UnityEngine;
using System.Collections;

public class CameraPage : MonoBehaviour
{
    public SliderEntryBinder fovBinder;
    public SliderEntryBinder flySpeedBinder;
    public SliderEntryBinder speedUpBinder;
    public SliderEntryBinder speedDnBinder;
    
    void Awake()
    {
        fovBinder.SetConfigEntry(MeatKitPlugin.cfgCameraFov);
        flySpeedBinder.SetConfigEntry(MeatKitPlugin.cfgCameraFlySpeed);
        speedUpBinder.SetConfigEntry(MeatKitPlugin.cfgCameraFlyFastMult);
        speedDnBinder.SetConfigEntry(MeatKitPlugin.cfgCameraFlySlowMult);
    }
}
