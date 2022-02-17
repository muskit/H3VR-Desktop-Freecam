using System.Collections;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class SliderEntryBinder : MonoBehaviour {
    public Slider slider;
    public InputField inputField;
    public bool clampValue;

    public ConfigEntry<float> cfgVal;

    void Awake () {
        if (slider == null || inputField == null)
        {
            Debug.LogError("Slider/input field references missing, cannot bind!");
            return;
        }

        slider.onValueChanged.AddListener(OnSliderChange);
        inputField.onEndEdit.AddListener(OnInputFieldDoneEditing);
    }

    public void SetConfigEntry(ConfigEntry<float> newCfg)
    {
        cfgVal = newCfg;
        slider.value = cfgVal.Value;
        inputField.text = cfgVal.Value.ToString("F");
    }

    public void OnSliderChange(float newValue)
    {
        if (cfgVal != null)
            cfgVal.Value = newValue;
        inputField.text = newValue.ToString("F");
    }

    public void OnInputFieldDoneEditing(string newText)
    {
        try
        {
            float value = float.Parse(newText);
            if(clampValue)
                value = Mathf.Clamp(value, slider.minValue, slider.maxValue);

            if (cfgVal != null)
                cfgVal.Value = value;

            inputField.text = value.ToString("F");
            slider.value = value;
        }
        catch
        {
            if (cfgVal != null)
                inputField.text = cfgVal.Value.ToString("F");
            else
                inputField.text = slider.value.ToString("F");
        }
    }
}
