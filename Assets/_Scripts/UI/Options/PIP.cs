using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PIP : MonoBehaviour {
    public Toggle tglMovingAndScaling;
    public SliderEntryBinder fovBinder;
    
	void Awake () {
        fovBinder.SetConfigEntry(MeatKitPlugin.cfgPipFov);

        tglMovingAndScaling.isOn = MeatKitPlugin.cfgPipEditWindow.Value;
        tglMovingAndScaling.onValueChanged.AddListener(ToggleViewEdit);
	}
	
    private void ToggleViewEdit(bool newVal)
    {
        MeatKitPlugin.cfgPipEditWindow.Value = newVal;
    }
}
