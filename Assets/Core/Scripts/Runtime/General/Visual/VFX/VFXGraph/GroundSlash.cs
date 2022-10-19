using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GroundSlash : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed;
    [SerializeField] private GameObject trail;
    private VisualEffect visualEffect;
    private float elapsedTime;
    private float duration;
    private string lifeTImeProperty = "SlashLifetime";
    private PlayerController playerController;

    #endregion

    #region general

    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
        duration = visualEffect.GetFloat(lifeTImeProperty);
    }

    private void Start()
    {
        elapsedTime = 0;
        playerController = PlayerController.Instance;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime <= duration)
        {
            if (elapsedTime >= duration - 0.3f)
            {
                trail.SetActive(false);
            }
            transform.position = transform.position + transform.forward * speed;
        }
        else
        {
            AssetLoader.ReleaseAssetInstance(gameObject);
        }         
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.PlayerTarget.DoDamage(100, 0);
            playerController.TriggerDOT(10, 4);
            Debug.Log("Damaged !");
        }
    }

    #endregion
}
