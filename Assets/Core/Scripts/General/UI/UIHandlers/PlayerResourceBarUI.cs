using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceBarUI : MonoBehaviour
{
    [SerializeField] private float updateSpeedInSec = 0.5f;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image manaBarImage;
    private PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        SupscribeEvent(player);
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        UnSunbscribeEvent(player);
    }

    private void SupscribeEvent(PlayerController playerCon)
    {
        playerCon.PlayerTarget.Resource.OnHealthLoss += Resource_OnHealthLoss;
        playerCon.PlayerTarget.Resource.OnHealthGain += Resource_OnHealthGain;
        playerCon.PlayerTarget.Resource.OnManaLoss += Resource_OnManaLoss;
        playerCon.PlayerTarget.Resource.OnManaGain += Resource_OnManaGain;
        playerCon.PlayerTarget.Resource.OnKilled += Resource_OnKilled;
    }

    private void UnSunbscribeEvent(PlayerController playerCon)
    {
        playerCon.PlayerTarget.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        playerCon.PlayerTarget.Resource.OnHealthGain -= Resource_OnHealthGain;
        playerCon.PlayerTarget.Resource.OnManaLoss -= Resource_OnManaLoss;
        playerCon.PlayerTarget.Resource.OnManaGain -= Resource_OnManaGain;
        playerCon.PlayerTarget.Resource.OnKilled -= Resource_OnKilled;
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

    private void SceneLoader_OnNewGameStart()
    {
        player = PlayerController.Instance;
        SupscribeEvent(player);
        healthBarImage.fillAmount = 1;
        manaBarImage.fillAmount = 1;
    }

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
        UnSunbscribeEvent(player);
    }
}
