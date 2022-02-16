using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class IntEvent : UnityEvent<int> { }

    public class TabButton : MonoBehaviour
    {

        public GameObject panel;
        public IntEvent onTabClick = new IntEvent();

        private static readonly Color colorActivated = new Color32(0x5C, 0x5C, 0x5C, 0xFF);
        private static readonly Color colorDeactivated = new Color32(0x8B, 0x8D, 0x8E, 0xFF);

        public void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void SetState(bool val)
        {
            if (val)
            {
                GetComponent<Image>().color = colorActivated;
            }
            else
            {
                GetComponent<Image>().color = colorDeactivated;
            }
            panel.SetActive(val);
        }

        // emit event for TabGroup
        public void OnClick()
        {
            onTabClick.Invoke(transform.GetSiblingIndex());
        }
    }
}