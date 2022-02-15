using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
	public class MasterUI : MonoBehaviour
	{
		// Controllable objects
		public static GameObject freecam;

		// UI elements
		private Button uiToggleButton;
        private Button uiOptionsButton;
		private Slider uiMouseSpeedSlider;
		private InputField uiMouseSpeedEntry;

        // extra windows
        private GameObject uiOptionsWindow;

		private bool desktopEnabled = false;

		// Use this for initialization
		void Start()
		{
			uiToggleButton = transform.GetChild(0).GetChild(0).GetComponent<Button>();
			uiToggleButton.onClick.AddListener(ToggleDesktop);

            uiOptionsButton = transform.GetChild(0).GetChild(1).GetComponent<Button>();

            uiOptionsWindow = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Settings"));

            uiMouseSpeedSlider = transform.FindDeepChild("MouseSpeed Slider").GetComponent<Slider>();
			uiMouseSpeedSlider.onValueChanged.AddListener(OnMouseSpeedSliderChg);
			uiMouseSpeedEntry = transform.FindDeepChild("MouseSpeed Entry").GetComponent<InputField>();
			uiMouseSpeedEntry.onEndEdit.AddListener(OnMouseSpeedEntryDoneEditing);
			uiMouseSpeedEntry.text = MeatKitPlugin.mouseSpeed.ToString("F");
			uiMouseSpeedSlider.value = MeatKitPlugin.mouseSpeed;
            
			SetDesktopUIEnable(false);
		}

		public void SetDesktopUIEnable(bool val)
		{
			desktopEnabled = val;
			Text btnText = uiToggleButton.GetComponentInChildren<Text>();
			if (desktopEnabled)
			{
				uiMouseSpeedSlider.gameObject.SetActive(true);
				uiMouseSpeedEntry.gameObject.SetActive(true);
                freecam = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Freecam"));
                DontDestroyOnLoad(freecam);
                btnText.text = "Disable Desktop";
			}
			else
			{
				uiMouseSpeedSlider.gameObject.SetActive(false);
				uiMouseSpeedEntry.gameObject.SetActive(false);
				Destroy(freecam);
				freecam = null;
				btnText.text = "Enable Desktop";
			}
		}

		private void ToggleDesktop()
        {
			SetDesktopUIEnable(!desktopEnabled);
        }

		public void OnMouseSpeedSliderChg(float newValue)
        {
			MeatKitPlugin.mouseSpeed = newValue;
			uiMouseSpeedEntry.text = newValue.ToString("F");
		}
		
		public void OnMouseSpeedEntryDoneEditing(string newText)
        {
			try
			{
				float value = float.Parse(newText);
				value = Mathf.Clamp(value, uiMouseSpeedSlider.minValue, uiMouseSpeedSlider.maxValue);
				MeatKitPlugin.mouseSpeed = value;

				uiMouseSpeedEntry.text = value.ToString("F");
				uiMouseSpeedSlider.value = value;
			}
			catch
			{
				uiMouseSpeedEntry.text = uiMouseSpeedSlider.value.ToString("F");
			}
		}
	}
}