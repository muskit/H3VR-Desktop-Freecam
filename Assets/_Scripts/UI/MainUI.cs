using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
	public class MainUI : MonoBehaviour
	{
		public static GameObject freecam;

		// UI elements
		public Button uiToggleButton;
        public Button uiOptionsButton;

        // extra windows
        private GameObject uiOptionsWindow;

		private bool freecamEnabled = false;
        private bool uiOptionsEnabled = false;

		// Use this for initialization
		void Awake()
		{
			uiToggleButton.onClick.AddListener(ToggleFreecam);
            uiOptionsButton.onClick.AddListener(ToggleOptionsUI);
            
			SetFreecamEnable(false);
		}

		public void SetFreecamEnable(bool val)
		{
			freecamEnabled = val;
			Text btnText = uiToggleButton.GetComponentInChildren<Text>();
			if (freecamEnabled)
			{
                freecam = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Freecam"));
                DontDestroyOnLoad(freecam);
                btnText.text = "Disable Freecam";
			}
			else
			{
				Destroy(freecam);
				//freecam = null; // unnecessary?
				btnText.text = "Enable Freecam";
			}
		}

		private void ToggleFreecam()
        {
			SetFreecamEnable(!freecamEnabled);
        }

        private void ToggleOptionsUI()
        {
            if (uiOptionsWindow == null)
            {
                uiOptionsWindow = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Options"));
                DontDestroyOnLoad(uiOptionsWindow);
            }
            else
            {
                Destroy(uiOptionsWindow);
            }
        }
	}
}