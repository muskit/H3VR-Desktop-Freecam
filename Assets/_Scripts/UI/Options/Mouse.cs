using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class Mouse : MonoBehaviour
    {
        public Toggle tglPitchReverse;
        public Toggle tglYawReverse;
        public SliderEntryBinder sensitivityBinder;
        
        void Awake()
        {
            sensitivityBinder.SetConfigEntry(MeatKitPlugin.cfgMouseSensitivity);

            tglPitchReverse.onValueChanged.AddListener(TogglePitchReverse);
            tglPitchReverse.isOn = MeatKitPlugin.cfgMousePitchFlip.Value;

            tglYawReverse.onValueChanged.AddListener(ToggleYawReverse);
            tglYawReverse.isOn = MeatKitPlugin.cfgMouseYawFlip.Value;
        }

        private void TogglePitchReverse(bool newVal)
        {
            MeatKitPlugin.cfgMousePitchFlip.Value = newVal;
        }

        private void ToggleYawReverse(bool newVal)
        {
            MeatKitPlugin.cfgMouseYawFlip.Value = newVal;
        }
    }
}