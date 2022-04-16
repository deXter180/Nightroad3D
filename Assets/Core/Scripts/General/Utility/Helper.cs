using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    private static PlayerInputAsset inputAction;
    private static Camera cam;
    public static Camera MainCam
    {
        get
        {
            if (cam == null) cam = Camera.main;
            return cam;
        }
    }

    private static Dictionary<float, WaitForSeconds> WaitDict = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDict.TryGetValue(time, out var wait)) return wait;
        WaitDict[time] = new WaitForSeconds(time);
        return WaitDict[time];
    }

    private static PointerEventData eventDataCurrentPos;
    private static List<RaycastResult> results;
    public static bool IsOverUI()
    {
        if (inputAction == null) inputAction = InputManager.InputActions;
        eventDataCurrentPos = new PointerEventData(EventSystem.current) { position = inputAction.BasicControls.MousePosition.ReadValue<Vector2>() };
        EventSystem.current.RaycastAll(eventDataCurrentPos, results);
        return results.Count > 0;
    }

    public static Vector2 GetWorldPosofCanvasElement(RectTransform transform)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform, transform.position, cam, out var result);
        return result;
    }

    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
}
