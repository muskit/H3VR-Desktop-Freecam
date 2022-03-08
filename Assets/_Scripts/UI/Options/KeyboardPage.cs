using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    enum UIKeybindState
    {
        None, Selected, Rebinding
    }

    public class KeyboardPage : MonoBehaviour
    {
        private static readonly System.Array keycodes = System.Enum.GetValues(typeof(KeyCode));

        // Keybind list
        private List<Transform> keybindEntries = new List<Transform>();
        private int curBindIdx = -1;
        
        // Rebinding trigger
        private float doubleClickTime = 0f;
        private bool isRebinding = false;

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
            SelectKeybind(-1);
        }
        
        // Event method
        private void OnKeybindClick(int idx)
        {
            if (isRebinding)
                return;

            if (idx == curBindIdx && Time.unscaledTime < doubleClickTime)
            {
                BeginRebinding(idx);
            }
            else
            {
                SelectKeybind(idx);
            }
            doubleClickTime = Time.unscaledTime + 0.5f;
        }

        // -1: clear all selections
        public void SelectKeybind(int idx)
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
                    UISetKeybindState(i, UIKeybindState.None);
                }
                curBindIdx = -1;
                return;
            }
            
            if (curBindIdx >= 0)
                UISetKeybindState(curBindIdx, UIKeybindState.None); // deselect old binding
            UISetKeybindState(idx, UIKeybindState.Selected); // select new binding
            curBindIdx = idx;
        }

        private void BeginRebinding(int idx)
        {
            UISetKeybindState(idx, UIKeybindState.Rebinding);
            Cursor.lockState = CursorLockMode.Locked;
            isRebinding = true;
        }

        private void EndRebinding()
        {
            UISetKeybindState(curBindIdx, UIKeybindState.Selected);
            Cursor.lockState = CursorLockMode.None;
            isRebinding = false;
        }

        private void UISetKeybindState(int idx, UIKeybindState state)
        {
            switch (state)
            {
                case UIKeybindState.None:
                    keybindEntries[idx].GetChild(0).gameObject.SetActive(false);
                    keybindEntries[idx].GetChild(2).GetChild(0).gameObject.SetActive(false);
                    keybindEntries[idx].GetChild(1).GetComponent<Text>().color = Color.white;
                    keybindEntries[idx].GetChild(2).GetComponent<Text>().color = Color.white;
                    break;
                case UIKeybindState.Selected:
                    keybindEntries[idx].GetChild(0).gameObject.SetActive(true);
                    keybindEntries[idx].GetChild(2).GetChild(0).gameObject.SetActive(false);
                    keybindEntries[idx].GetChild(1).GetComponent<Text>().color = Color.black;
                    keybindEntries[idx].GetChild(2).GetComponent<Text>().color = Color.black;
                    break;
                case UIKeybindState.Rebinding:
                    keybindEntries[idx].GetChild(0).gameObject.SetActive(false);
                    keybindEntries[idx].GetChild(2).GetChild(0).gameObject.SetActive(true);
                    keybindEntries[idx].GetChild(1).GetComponent<Text>().color = Color.white;
                    keybindEntries[idx].GetChild(2).GetComponent<Text>().color = Color.clear;
                    break;
            }
        }
        
        private void Update()
        {
            if (isRebinding)
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode code in keycodes)
                    {
                        if (Input.GetKeyDown(code))
                        {
                            if (code != KeyCode.Escape)
                            {
                                KBControls control = keybindEntries[curBindIdx].GetComponent<Keybind>().control;
                                Settings.cfgKeyboard[control].Value = code;
                            }
                            EndRebinding();
                            break;
                        }
                    }
                }
            }
        }
    }
}