using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DesktopFreecam
{
    public enum KBControls
    {
        MoveForward, MoveBackward, MoveLeft, MoveRight,
        Ascend, Descend,
        SpeedUp, SlowDown,
        TeleportToPlayer, ToggleControls, ToggleUI
    }

    public class Keybind : MonoBehaviour
    {
        public KBControls control;
        public Text keycodeDisplay;

        private void Update()
        {
            keycodeDisplay.text = Settings.cfgKeyboard[control].Value.ToString();
        }
    }
}
