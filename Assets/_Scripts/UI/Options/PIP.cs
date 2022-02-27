using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResXChanged<InputField> : UnityEvent { }

public class PIP : MonoBehaviour {
    public Toggle tglMovingAndScaling;
    public SliderEntryBinder fovBinder;
    public SliderEntryBinder opacityBinder;

    public InputField resolutionX;
    public InputField resolutionY;
    
	void Awake () {
        tglMovingAndScaling.isOn = MeatKitPlugin.cfgPipEditWindow.Value;
        tglMovingAndScaling.onValueChanged.AddListener(ToggleViewEdit);

        fovBinder.SetConfigEntry(MeatKitPlugin.cfgPipFov);
        opacityBinder.SetConfigEntry(MeatKitPlugin.cfgPipWindowOpacity);

        resolutionX.text = MeatKitPlugin.cfgPipResX.Value.ToString();
        resolutionX.onEndEdit.AddListener(OnResXDoneEditing);
        resolutionY.text = MeatKitPlugin.cfgPipResY.Value.ToString();
        resolutionY.onEndEdit.AddListener(OnResYDoneEditing);
	}
	
    private void OnResXDoneEditing(string msg)
    {
        int res;
        try
        {
            res = int.Parse(msg);
            if (res <= 0)
                throw new Exception();
        }
        catch (Exception _)
        {
            resolutionX.text = MeatKitPlugin.cfgPipResX.Value.ToString();
            return;
        }
        MeatKitPlugin.cfgPipResX.Value = res;
    }

    private void OnResYDoneEditing(string msg)
    {
        int res;
        try
        {
            res = int.Parse(msg);
            if (res <= 0)
                throw new Exception();
        }
        catch (System.Exception _)
        {
            resolutionY.text = MeatKitPlugin.cfgPipResY.Value.ToString();
            return;
        }
        MeatKitPlugin.cfgPipResY.Value = res;
    }

    private void ToggleViewEdit(bool newVal)
    {
        MeatKitPlugin.cfgPipEditWindow.Value = newVal;
    }
}
