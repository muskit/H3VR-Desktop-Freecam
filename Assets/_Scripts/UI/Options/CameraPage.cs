using UnityEngine;
using System.Collections;

namespace DesktopFreecam
{
    public class CameraPage : MonoBehaviour
    {
        public SliderEntryBinder fovBinder;
        public SliderEntryBinder flySpeedBinder;
        public SliderEntryBinder speedUpBinder;
        public SliderEntryBinder speedDnBinder;

        void Awake()
        {
            fovBinder.SetConfigEntry(Settings.cfgCameraFov);
            flySpeedBinder.SetConfigEntry(Settings.cfgCameraFlySpeed);
            speedUpBinder.SetConfigEntry(Settings.cfgCameraFlyFastMult);
            speedDnBinder.SetConfigEntry(Settings.cfgCameraFlySlowMult);
        }
    }
}