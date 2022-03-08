using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class LocationsPage : MonoBehaviour
    {
        public Dropdown sceneSelector;

        // Use this for initialization
        void Awake()
        {
            sceneSelector.AddOptions(new List<string>(Util.GetScenes()));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}