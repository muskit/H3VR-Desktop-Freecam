using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesktopFreecam
{
    // Attached to the parent of one or more TabButtons
    public class TabGroup : MonoBehaviour
    {

        private List<TabButton> tabButtons = new List<TabButton>();
        private int currentTabIdx = -1;

        // Use this for initialization
        void Start()
        {
            foreach (Transform child in transform)
            {
                var curTab = child.GetComponent<TabButton>();
                if (curTab != null)
                {
                    tabButtons.Add(curTab);
                    curTab.onTabClick.AddListener(OnTabClick);
                    curTab.SetState(false);
                }
            }

            if (tabButtons.Count > 0)
            {
                SwitchTo(0);
            }
        }

        private void OnTabClick(int idx)
        {
            SwitchTo(idx);
        }

        private void SwitchTo(int idx)
        {
            if (idx < 0 || idx >= tabButtons.Count)
                return;

            if (currentTabIdx >= 0)
                tabButtons[currentTabIdx].SetState(false);

            tabButtons[idx].SetState(true);
            currentTabIdx = idx;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}