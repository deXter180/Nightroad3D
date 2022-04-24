using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceBarUI : MonoBehaviour
{
    [SerializeField] private float updateSpeedInSec = 0.5f;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image manaBarImage;

    private void Start()
    {
        PlayerController.Instance.PlayerTarget.Resource.OnHealthLoss += Resource_OnHealthLoss;
        PlayerController.Instance.PlayerTarget.Resource.OnHealthGain += Resource_OnHealthGain;
        PlayerController.Instance.PlayerTarget.Resource.OnManaLoss += Resource_OnManaLoss;
        PlayerController.Instance.PlayerTarget.Resource.OnManaGain += Resource_OnManaGain;
        PlayerController.Instance.PlayerTarget.Resource.OnKilled += Resource_OnKilled;
    }

    private IEnumerator FillBar(Image barImage, float pct)
    {
        float preChangePct = barImage.fillAmount;
        float elapsed = 0f;

        while(elapsed < updateSpeedInSec)
        {
            elapsed += Time.deltaTime;
            barImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedInSec);
            yield return null;
        }
        barImage.fillAmount = pct;
    }


    //~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~

    private void Resource_OnHealthGain(object sender, ResourceManagement.DamagedEventArgs e)
    {
        StartCoroutine(FillBar(healthBarImage, e.currentHealthPct));
    }

    private void Resource_OnManaGain(object sender, ResourceManagement.ManaEventArgs e)
    {
        StartCoroutine(FillBar(manaBarImage, e.currentManaPct));
    }

    private void Resource_OnHealthLoss(object sender, ResourceManagement.DamagedEventArgs e)
    {
        StartCoroutine(FillBar(healthBarImage, e.currentHealthPct));
    }

    private void Resource_OnManaLoss(object sender, ResourceManagement.ManaEventArgs e)
    {
        StartCoroutine(FillBar(manaBarImage, e.currentManaPct));
    }

    private void Resource_OnKilled(object sender, System.EventArgs e)
    {
        PlayerController.Instance.PlayerTarget.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        PlayerController.Instance.PlayerTarget.Resource.OnHealthGain -= Resource_OnHealthGain;
        PlayerController.Instance.PlayerTarget.Resource.OnManaLoss -= Resource_OnManaLoss;
        PlayerController.Instance.PlayerTarget.Resource.OnManaGain -= Resource_OnManaGain;
        PlayerController.Instance.PlayerTarget.Resource.OnKilled -= Resource_OnKilled;
    }
}
