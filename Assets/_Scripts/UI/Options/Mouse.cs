using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class Mouse : MonoBehaviour
    {
        public Toggle tglPitchReverse;
        public Toggle tglYawReverse;
        public Slider sensSlider;
        public InputField sensEntry;

        // Use this for initialization
        void Start()
        {
            sensSlider.onValueChanged.AddListener(OnMouseSpeedSliderChg);
            sensSlider.value = MeatKitPlugin.cfgMouseSpeed.Value;
            sensEntry.onEndEdit.AddListener(OnMouseSpeedEntryDoneEditing);
            sensEntry.text = MeatKitPlugin.cfgMouseSpeed.Value.ToString("F");
        }

        public void OnMouseSpeedSliderChg(float newValue)
        {
            MeatKitPlugin.cfgMouseSpeed.Value = newValue;
            sensEntry.text = newValue.ToString("F");
        }

        public void OnMouseSpeedEntryDoneEditing(string newText)
        {
            try
            {
                float value = float.Parse(newText);
                value = Mathf.Clamp(value, sensSlider.minValue, sensSlider.maxValue);
                MeatKitPlugin.cfgMouseSpeed.Value = value;

                sensEntry.text = value.ToString("F");
                sensSlider.value = value;
            }
            catch
            {
                sensEntry.text = sensSlider.value.ToString("F");
            }
        }
    }
}