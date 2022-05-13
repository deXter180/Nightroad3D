using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    private PlayerInputAsset inputs;
    private bool isLocked;

    private void Start()
    {
        isLocked = false;
        StartCoroutine(InputDone());
    }

    private void Update()
    {
        if (inputs != null)
        {
            if (inputs.UI.RotateItem.triggered)
            {
                ControlCursor();
            }
        }        
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
    }

    private void ControlCursor()
    {
        if (!isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isLocked = false;
        }
    }
}
