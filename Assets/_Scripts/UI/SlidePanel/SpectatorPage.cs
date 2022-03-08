using System;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace DesktopFreecam {
    public class SpectatorPage : MonoBehaviour
    {

        [SerializeField]
        private SliderEntryBinder fovBinder;
        [SerializeField]
        private InputField fovEntry;
        [SerializeField]
        private Slider fovSlider;

        [SerializeField]
        private Dropdown specMode;

        void Awake()
        {
            fovBinder.SetState(Settings.specFov);
            fovBinder.ValueChanged += OnFOVChanged;

            specMode.onValueChanged.AddListener(OnModeChange);
        }

        // trig by fovBinder
        private void OnFOVChanged(object sender, EventArgs e)
        {
            fovEntry.text = Settings.specFov.Value.ToString("F");
            //fovSlider.value = Settings.specFov.Value;
        }

        // trig by specMode
        private void OnModeChange(int idx)
        {
            Util.FVRSetSpectatorMode((ControlOptions.DesktopCameraMode)idx);
        }

        void Update()
        {
            specMode.value = (int)GM.Options.ControlOptions.CamMode;
        }
    }
}