using System;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public enum ScrollWheelMode { MoveSpeed, FieldOfView }

    public class Mouse : MonoBehaviour
    {
        public Toggle tglPitchReverse;
        public Toggle tglYawReverse;
        public Dropdown ddScrollMode;
        public SliderEntryBinder sensitivityBinder;
        
        void Awake()
        {
            tglPitchReverse.onValueChanged.AddListener(TogglePitchReverse);
            tglPitchReverse.isOn = MeatKitPlugin.cfgMousePitchFlip.Value;

            tglYawReverse.onValueChanged.AddListener(ToggleYawReverse);
            tglYawReverse.isOn = MeatKitPlugin.cfgMouseYawFlip.Value;

            sensitivityBinder.SetConfigEntry(MeatKitPlugin.cfgMouseSensitivity);

            ddScrollMode.value = (int)MeatKitPlugin.cfgScrollMode.Value;
            ddScrollMode.onValueChanged.AddListener(SetWheelMode);
        }

        private void TogglePitchReverse(bool newVal)
        {
            MeatKitPlugin.cfgMousePitchFlip.Value = newVal;
        }

        private void ToggleYawReverse(bool newVal)
        {
            MeatKitPlugin.cfgMouseYawFlip.Value = newVal;
        }

        private void SetWheelMode(int idx)
        {
            MeatKitPlugin.cfgScrollMode.Value = (ScrollWheelMode)idx;
        }
    }
}