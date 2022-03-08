using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class FreecamPage : MonoBehaviour
    {
        [SerializeField]
        private SliderEntryBinder fovBinder;
        [SerializeField]
        private SliderEntryBinder speedBinder;
        [SerializeField]
        private Toggle tglIsVisibleToPlayer;

        // Use this for initialization
        void Awake()
        {
            fovBinder.SetConfigEntry(Settings.cfgCameraFov);
            speedBinder.SetConfigEntry(Settings.cfgCameraFlySpeed);

            tglIsVisibleToPlayer.isOn = Settings.freecamVisibleToPlayer.Value;
            tglIsVisibleToPlayer.onValueChanged.AddListener(SetVRVisibility);
        }

        private void SetVRVisibility(bool value)
        {
            Settings.freecamVisibleToPlayer.Value = value;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}