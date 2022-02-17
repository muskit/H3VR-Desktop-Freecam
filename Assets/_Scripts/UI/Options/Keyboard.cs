using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    public class Keyboard : MonoBehaviour
    {

        private static readonly System.Array keycodes = System.Enum.GetValues(typeof(KeyCode));

        // Keybind list
        private List<Transform> keybindEntries = new List<Transform>();
        private int curBindIdx = -1;

        // Use this for initialization
        private void Start()
        {
            int btnIdx = 0;
            foreach (Button btn in transform.GetChild(0).GetComponentsInChildren<Button>())
            {
                int i = btnIdx;
                btn.onClick.AddListener(delegate { OnKeybindClick(i); });
                ++btnIdx;
                keybindEntries.Add(btn.transform);
            }
        }

        // update panel here as well
        private void OnKeybindClick(int idx)
        {
            Debug.Log("Clicked binding " + idx);
            UIChooseKeybind(idx);
        }

        public void UIChooseKeybind(int idx)
        {
            if (idx >= keybindEntries.Count || idx < -1)
            {
                Debug.LogWarning("Attempted to select invalid keybind.");
                return;
            }

            // reset selection
            if (idx == -1)
            {
                for (int i = 0; i < keybindEntries.Count; ++i)
                {
                    UISetKeybindState(i, false);
                }
                return;
            }

            if (curBindIdx > -1)
            {
                UISetKeybindState(curBindIdx, false);
            }

            UISetKeybindState(idx, true);
            curBindIdx = idx;
        }

        private void UISetKeybindState(int idx, bool enabled)
        {
            keybindEntries[idx].GetChild(0).gameObject.SetActive(enabled);
            keybindEntries[idx].GetChild(1).GetComponent<Text>().color = enabled ? Color.black : Color.white;
            keybindEntries[idx].GetChild(2).GetComponent<Text>().color = enabled ? Color.black : Color.white;
        }

        // Update is called once per frame
        private void Update()
        {
            // for input rebinding
            if (Input.anyKeyDown)
            {
                foreach (KeyCode code in keycodes)
                {
                    if (Input.GetKeyDown(code))
                    {
                        // TODO: rebind selected binding
                    }
                }
            }
        }
    }
}