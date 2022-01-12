using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Yielders
{
    private class FloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y)
        {
            return x == y;
        }

        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }
    private static Dictionary<float, WaitForSeconds> SecondWaitDict = new Dictionary<float, WaitForSeconds>(5000, new FloatComparer());

    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        WaitForSeconds waiter;
        if (!SecondWaitDict.TryGetValue(seconds, out waiter))
        {
            SecondWaitDict.Add(seconds, waiter = new WaitForSeconds(seconds));
        }
        return waiter;
    }
}
