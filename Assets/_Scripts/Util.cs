using UnityEngine;
using FistVR;

public class Util
{
    public static Color InvertColor(Color col)
    {
        return new Color(1 - col.r, 1 - col.g, 1 - col.b, col.a);
    }

    public static void SetDesktopMode(ControlOptions.DesktopCameraMode mode)
    {
        var specPanel = Object.FindObjectOfType<SpectatorPanel>();
        if (specPanel != null)
        {
            specPanel.BTN_SetCamMode((int)mode);
        }
        else
            GM.Options.ControlOptions.CamMode = mode;
    }
}
