using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class ResXChanged<InputField> : UnityEvent { }

    public class PIPPage : MonoBehaviour
    {
        public Toggle tglMovingAndScaling;
        public SliderEntryBinder fovBinder;
        public SliderEntryBinder opacityBinder;

        public InputField resolutionX;
        public InputField resolutionY;

        void Awake()
        {
            tglMovingAndScaling.isOn = Settings.specWindowDraggable.Value;
            tglMovingAndScaling.onValueChanged.AddListener(SetViewEdit);

            fovBinder.SetState(Settings.specFov);
            opacityBinder.SetConfigEntry(Settings.cfgPipOpacity);

            resolutionX.text = Settings.cfgPipResX.Value.ToString();
            resolutionX.onEndEdit.AddListener(OnResXDoneEditing);
            resolutionY.text = Settings.cfgPipResY.Value.ToString();
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
                resolutionX.text = Settings.cfgPipResX.Value.ToString();
                return;
            }
            Settings.cfgPipResX.Value = res;
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
                resolutionY.text = Settings.cfgPipResY.Value.ToString();
                return;
            }
            Settings.cfgPipResY.Value = res;
        }

        private void SetViewEdit(bool newVal)
        {
            Settings.specWindowDraggable.Value = newVal;
        }
    }
}