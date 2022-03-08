using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
	public class MainUI : MonoBehaviour
	{
		public static GameObject manager;

        private bool desktopEnabled = false;

		// UI elements
		public Button uiToggleButton;
        public Button uiOptionsButton;

        // extra windows
        private GameObject uiOptionsWindow;

		// Use this for initialization
		void Awake()
		{
			uiToggleButton.onClick.AddListener(ToggleFreecam);
            uiOptionsButton.onClick.AddListener(ToggleOptionsUI);
            
			SetFreecamEnable(false);
		}

		public void SetFreecamEnable(bool val)
		{
			desktopEnabled = val;
			Text btnText = uiToggleButton.GetComponentInChildren<Text>();
			if (desktopEnabled)
			{
                manager = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("SpectatorManager"));
                DontDestroyOnLoad(manager);

                btnText.text = "Disable";
			}
			else
			{
				Destroy(manager);
				btnText.text = "Enable";
			}
		}

		private void ToggleFreecam()
        {
			SetFreecamEnable(!desktopEnabled);
        }

        private void ToggleOptionsUI()
        {
            if (uiOptionsWindow == null)
            {
                uiOptionsWindow = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("Options"), transform);
                DontDestroyOnLoad(uiOptionsWindow);
            }
            else
            {
                Destroy(uiOptionsWindow);
            }
        }

        private void Update()
        {
            if (desktopEnabled)
            {
                
            }
            else
            {

            }
        }
    }
}