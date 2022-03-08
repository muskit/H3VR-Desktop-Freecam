using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DesktopFreecam;

public class SlidePanel : MonoBehaviour {
    public Toggle pipEnabled;
    public Toggle pipUnlocked;
    public Button viewModeBtn;

    private void Awake()
    {
        pipEnabled.isOn = Settings.pipEnabled.Value;
        pipEnabled.onValueChanged.AddListener(SetPIPState);
        
        pipUnlocked.isOn = Settings.cfgPipUnlocked.Value;
        pipUnlocked.onValueChanged.AddListener(SetPIPUnlock);
        
        viewModeBtn.onClick.AddListener(OnModeBtnPress);
        UpdateModeButtonText();
    }

    private void SetPIPState(bool value)
    {
        Settings.pipEnabled.Value = value;
    }

    private void SetPIPUnlock(bool value)
    {
        Settings.cfgPipUnlocked.Value = value;
    }
    
    // trig by viewModeBtn
    private void OnModeBtnPress()
    {
        var modeArray = Enum.GetValues(typeof(SpectatorMode));
        int idx = (int)Settings.mainViewMode.Value;
        idx = (idx + 1) % modeArray.Length;
        Settings.mainViewMode.Value = (SpectatorMode)idx;
        UpdateModeButtonText();
    }

    private void UpdateModeButtonText()
    {
        string type = "";
        switch (Settings.mainViewMode.Value)
        {
            case SpectatorMode.FVRSpectator:
                type = "Spectator";
                break;
            case SpectatorMode.Freecam:
                type = "Freecam";
                break;
        }
        viewModeBtn.GetComponentInChildren<Text>().text = "Main view: " + type;
    }
    
    void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnDestroy()
    {
        pipEnabled.onValueChanged.RemoveListener(SetPIPState);
        pipUnlocked.onValueChanged.RemoveListener(SetPIPUnlock);
        viewModeBtn.onClick.RemoveListener(OnModeBtnPress);

    }
}
