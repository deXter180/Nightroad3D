using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverlay : MonoBehaviour
{    
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float fadeSpeed = 5f;
    private PlayerController player;
    private OverlayManager overlayManager;
    private float durationTimer;
    private int currentHealth;
    private int maxHealth;
    private float intensity;

    void Start()
    {
        player = PlayerController.Instance;
        overlayManager = OverlayManager.Instance;
        maxHealth = player.MaxHitPoints;
        player.PlayerTarget.Resource.OnHealthLoss += Resource_OnHealthLoss;
        player.PlayerTarget.Resource.OnHealthGain += Resource_OnHealthGain;
        intensity = 0;
    }

    private void OnDisable()
    {
        player.PlayerTarget.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        player.PlayerTarget.Resource.OnHealthGain -= Resource_OnHealthGain;
    }


    void Update()
    {
        FadeDamageEffect();
    }

    private void FadeDamageEffect()
    {
        if (GetHealthPct() < 0.3f)
        {
            //Add another effect
        }

        if (intensity > 0)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                intensity -= Time.deltaTime * fadeSpeed;
            }
        }
        else
        {
            overlayManager.RemoveOverlayEffect(OverlayTypes.Damage);
        }

        float GetHealthPct()
        {
            return currentHealth / maxHealth;
        }
    }

   

    //~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

    private void Resource_OnHealthGain(object sender, ResourceManagement.DamagedEventArgs e)
    {
        currentHealth = e.CurrentHP;
    }

    private void Resource_OnHealthLoss(object sender, ResourceManagement.DamagedEventArgs e)
    {
        currentHealth = e.CurrentHP;
        durationTimer = 0;
        intensity = 1f;
        overlayManager.AddOverlayEffect(OverlayTypes.Damage);
    }
}
