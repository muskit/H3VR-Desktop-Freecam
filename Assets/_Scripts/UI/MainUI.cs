using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
	public class MainUI : MonoBehaviour
	{
		// Controllable objects
		public static GameObject freecam;

		// UI elements
		private Button uiToggleButton;
        private Button uiOptionsButton;

        // extra windows
        private GameObject uiOptionsWindow;

		private bool freecamEnabled = false;
        private bool uiOptionsEnabled = false;

		// Use this for initialization
		void Start()
		{
			uiToggleButton = transform.GetChild(0).GetChild(0).GetComponent<Button>();
			uiToggleButton.onClick.AddListener(ToggleFreecam);

            uiOptionsButton = transform.GetChild(0).GetChild(1).GetComponent<Button>();
            uiOptionsButton.onClick.AddListener(OpenOptionsUI);
            
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
				freecam = null; // unnecessary?
				btnText.text = "Enable Freecam";
			}
		}

		private void ToggleFreecam()
        {
			SetFreecamEnable(!freecamEnabled);
        }

        private void OpenOptionsUI()
        {
            if (uiOptionsWindow == null)
                uiOptionsWindow = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Options"));
        }
	}
}