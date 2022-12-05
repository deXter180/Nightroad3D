using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour
{
    #region Variables

    [SerializeField] private string LevelToSwitch;
    private Collider thisCol;

    #endregion

    #region General

    private void Awake()
    {
        thisCol = GetComponent<Collider>();
        thisCol.isTrigger = true;
    }



    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            SceneLoader.LoadNewSingleScene(LevelToSwitch);
            GameController.Instance.DisableFPSCam();
        }       
    }

    #endregion
}
