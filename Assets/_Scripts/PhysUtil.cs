using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysUtil
{
    public static float GetRequiredVelocityChange(float aFinalSpeed, float aDrag)
    {
        float m = Mathf.Clamp01(aDrag * Time.fixedDeltaTime);
        return aFinalSpeed * m / (1 - m);
    }
    public static float GetRequiredAcceleraton(float aFinalSpeed, float aDrag)
    {
        return GetRequiredVelocityChange(aFinalSpeed, aDrag) / Time.fixedDeltaTime;
    }
}