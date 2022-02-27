using System;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class SliderEntryBinder : MonoBehaviour {
    public Slider slider; // Optional
    public InputField inputField;
    public bool clampToSlider;

    public ConfigEntry<float> cfgVal;

    void Awake () {
        //if (slider == null && inputField == null)
        //{
        //    Debug.LogError("Slider and input field references missing, cannot bind!");
        //    return;
        //}

        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderChange);
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnInputFieldDoneEditing);
    }

    public void SetConfigEntry(ConfigEntry<float> newCfg)
    {
        cfgVal = newCfg;

        // (if only Unity 5.6 used C# 6 where null-conditionals
        //  exist and I can make this cleaner!!)
        if (slider != null)
            slider.value = cfgVal.Value;
        if (inputField != null)
            inputField.text = cfgVal.Value.ToString("F");
        cfgVal.SettingChanged += OnConfigValChange;
    }

    public void OnConfigValChange(object sender, EventArgs e)
    {
        float newVal = ((ConfigEntry<float>)sender).Value;
        if (inputField != null)
        {
            inputField.text = newVal.ToString("F");
        }
        if (slider != null)
        {
            slider.value = newVal;
        }
    }

    public void OnSliderChange(float newValue)
    {
        if (cfgVal != null)
            cfgVal.Value = newValue;
        inputField.text = newValue.ToString("F");
    }

    public void OnInputFieldDoneEditing(string newText)
    {
        string oldText = inputField.text;
        try
        {
            float value = float.Parse(newText);
            
            if (slider != null)
            {
                slider.value = value;
                if (clampToSlider)
                    value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
            }

            if (cfgVal != null)
                cfgVal.Value = value;

            inputField.text = value.ToString("F");
        }
        catch
        {
            if (cfgVal != null)
                inputField.text = cfgVal.Value.ToString("F");
            else
                inputField.text = oldText;
        }
    }
}
