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
        CharacterAttribute.OnVITorSPRChanged += CharacterAttribute_OnVITorSPRChanged;
        player.PlayerTarget.Resource.OnHealthLoss += Resource_OnHealthLoss;
        player.PlayerTarget.Resource.OnHealthGain += Resource_OnHealthGain;
        player.onResettingHP += Player_onResettingHP;
        intensity = 0;
    }

    private void OnDisable()
    {
        CharacterAttribute.OnVITorSPRChanged -= CharacterAttribute_OnVITorSPRChanged;
        player.PlayerTarget.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        player.PlayerTarget.Resource.OnHealthGain -= Resource_OnHealthGain;
        player.onResettingHP -= Player_onResettingHP;
    }



    void Update()
    {
        if (maxHealth > 0)
        {
            FadeDamageEffect();
        }        
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

    private void CharacterAttribute_OnVITorSPRChanged(AttributeTypes type)
    {
        if (type == AttributeTypes.Vitality)
        {
            maxHealth = player.MaxHitPoints;
        }
    }

    private void Player_onResettingHP()
    {
        maxHealth = player.MaxHitPoints;
    }
}
