using System;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public enum ScrollWheelMode { MoveSpeed, FieldOfView }

    public class MousePage : MonoBehaviour
    {
        public Toggle tglPitchReverse;
        public Toggle tglYawReverse;
        public Dropdown ddScrollMode;
        public SliderEntryBinder sensitivityBinder;
        
        void Awake()
        {
            tglPitchReverse.onValueChanged.AddListener(TogglePitchReverse);
            tglPitchReverse.isOn = Settings.cfgMousePitchFlip.Value;

            tglYawReverse.onValueChanged.AddListener(ToggleYawReverse);
            tglYawReverse.isOn = Settings.cfgMouseYawFlip.Value;

            sensitivityBinder.SetConfigEntry(Settings.cfgMouseSensitivity);

            ddScrollMode.value = (int)Settings.cfgScrollMode.Value;
            ddScrollMode.onValueChanged.AddListener(SetWheelMode);
        }

        private void TogglePitchReverse(bool newVal)
        {
            Settings.cfgMousePitchFlip.Value = newVal;
        }

        private void ToggleYawReverse(bool newVal)
        {
            Settings.cfgMouseYawFlip.Value = newVal;
        }

        private void SetWheelMode(int idx)
        {
            Settings.cfgScrollMode.Value = (ScrollWheelMode)idx;
        }
    }
}