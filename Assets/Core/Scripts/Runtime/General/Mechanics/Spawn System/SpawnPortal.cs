using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnPortal : MonoBehaviour
{
    #region Variables

    private VisualEffect spawnVFX;
    private string spawnPropText = "SpawnRate";
    private float delayTime = 0;
    public int PortalIndex { get; set; }
    public event Action<SpawnPortal> OnDestroyPortal;

    #endregion

    #region General

    private void Awake()
    {
        spawnVFX = GetComponent<VisualEffect>();       
    }

    private void OnEnable()
    {
        delayTime = spawnVFX.GetFloat("Lifetime");
    }

    public IEnumerator PlaySpawnVFX(int amount)
    {       
        yield return Helpers.GetWait(0.5f);
        spawnVFX.SetFloat(spawnPropText, amount);
        spawnVFX.Play();
        StartCoroutine(DelayDisable());

        IEnumerator DelayDisable()
        {
            yield return Helpers.GetWait(delayTime);
            OnDestroyPortal?.Invoke(this);
            Destroy(gameObject);
        }
    }

    #endregion
}
