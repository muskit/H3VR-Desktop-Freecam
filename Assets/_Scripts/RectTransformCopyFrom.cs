using UnityEngine;

public static class RectTransformCopyFrom
{
    // https://forum.unity.com/threads/how-to-copy-a-recttransform.497791/
    /// <summary>
    /// Set this RectTransform's properties from another RectTransform.
    /// </summary>
    /// <param name="from">The RectTransform to get properties from.</param>
    public static void CopyFrom (this RectTransform to, RectTransform from)
    {
        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.anchoredPosition = from.anchoredPosition;
        to.sizeDelta = from.sizeDelta;
        to.pivot = from.pivot;
    }
}
