using System;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class SliderEntryBinder : MonoBehaviour {
    public EventHandler ValueChanged;

    public Slider slider; // Optional
    public InputField inputField;
    public bool clampToSlider;

    private ConfigEntry<float> config;
    private State<float> state;

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
        config = newCfg;
        state = null;

        UpdateUIValues(config.Value);
        config.SettingChanged += OnExternalChange;
    }

    public void SetState(State<float> newState)
    {
        state = newState;
        config = null;

        UpdateUIValues(state.Value);
        state.ValueChanged += OnExternalChange;
    }

    // Changed by the State/ConfigEntry being changed directly somewhere else
    public void OnExternalChange(object sender, EventArgs e)
    {
        float newVal;
        var type = sender.GetType();

        if (type == (typeof(ConfigEntry<float>)))
            newVal = ((ConfigEntry<float>)sender).Value;
        else if (type == (typeof(State<float>)))
            newVal = ((State<float>)sender).Value;
        else
            return;

        UpdateUIValues(newVal);
        InvokeValueChanged();
    }

    public void SetValue(float value)
    {
        SetVariables(value);
        UpdateUIValues(value);
        InvokeValueChanged();
    }

    public void OnSliderChange(float value)
    {
        inputField.text = value.ToString("F");
        SetVariables(value);
        InvokeValueChanged();
    }

    public void OnInputFieldDoneEditing(string text)
    {
        string oldText = inputField.text;
        try
        {
            float value = float.Parse(text);
            
            if (slider != null)
            {
                slider.value = value;
                if (clampToSlider)
                    value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
            }

            SetVariables(value);
            inputField.text = value.ToString("F");
            InvokeValueChanged();
        }
        catch
        {
            if (config != null)
                inputField.text = config.Value.ToString("F");
            else if (state != null)
                inputField.text = state.Value.ToString("F");
            else
                inputField.text = oldText;
        }
    }

    private void InvokeValueChanged()
    {
        if (ValueChanged != null)
            ValueChanged.Invoke(this, EventArgs.Empty);
    }

    private void SetVariables(float value)
    {
        if (config != null)
            config.Value = value;
        else if (state != null)
            state.Value = value;
    }

    private void UpdateUIValues(float value)
    {
        if (inputField != null)
        {
            inputField.text = value.ToString("F");
        }
        if (slider != null)
        {
            slider.value = value;
        }
    }
}
