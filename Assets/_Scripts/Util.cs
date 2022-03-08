using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using UnityEngine.SceneManagement;

public class Util
{
    public static void FVRSetSpectatorMode(ControlOptions.DesktopCameraMode mode)
    {
        var specPanel = Object.FindObjectOfType<SpectatorPanel>();
        if (specPanel != null)
        {
            specPanel.BTN_SetCamMode((int)mode);
        }
        else
            GM.Options.ControlOptions.CamMode = mode;
    }

    public static ControlOptions.DesktopCameraMode FVRGetSpectatorMode()
    {
        return GM.Options.ControlOptions.CamMode;
    }

    public static void FVRSetSpectatorFOV(float fov)
    {
        var specPanel = Object.FindObjectOfType<SpectatorPanel>();
        if (specPanel != null)
        {
            //specPanel.BTN_SetFOV(fov);
            specPanel.FOVLabel.text = fov.ToString();
        }
        GM.Options.ControlOptions.CamFOV = fov;
    }

    public static Color InvertColor(Color col)
    {
        return new Color(1 - col.r, 1 - col.g, 1 - col.b, col.a);
    }

    public static List<GameObject> SortByDistance(GameObject from, List<GameObject> list)
    {
        return list.OrderBy(
            x => (from.transform.position - x.transform.position).sqrMagnitude
            ).ToList();
    }

    public static string[] GetScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
        return scenes;
    }
}
