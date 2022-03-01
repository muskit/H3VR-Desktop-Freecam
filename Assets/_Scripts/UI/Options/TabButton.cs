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
        public bool invertTextColorOnActive;

        private Text btnText;
        private Color normalTextColor;

        //private static readonly Color colorActivated = new Color32(0x5C, 0x5C, 0x5C, 0xFF);
        //private static readonly Color colorDeactivated = new Color32(0x8B, 0x8D, 0x8E, 0xFF);

        public Color colorActivated;
        public Color colorDeactivated;

        public void Awake()
        {
            var btn = GetComponent<Button>();
            btnText = btn.transform.GetChild(0).GetComponent<Text>();

            btn.onClick.AddListener(OnClick);
            normalTextColor = btnText.color;

            colorActivated.a = GetComponent<Image>().color.a;
            colorDeactivated.a = GetComponent<Image>().color.a;
        }

        public void SetState(bool val)
        {
            if (val)
            {
                GetComponent<Image>().color = colorActivated;
                if (invertTextColorOnActive)
                    btnText.color = Util.InvertColor(normalTextColor);
            }
            else
            {
                GetComponent<Image>().color = colorDeactivated;
                if (invertTextColorOnActive)
                    btnText.color = normalTextColor;
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