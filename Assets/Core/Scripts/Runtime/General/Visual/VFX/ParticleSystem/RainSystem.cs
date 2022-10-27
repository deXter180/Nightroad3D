using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSystem : PersistentSingleton<RainSystem>
{
    #region SerializedVariables

    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private GameObject lightningFlash;
    [SerializeField] private float Xoffset = 0;
    [SerializeField] private float Zoffset = -10;

    #endregion

    #region Variables

    private int groundLayer = 1 << 8;
    private int waterLayer = 1 << 4;
    private int bitMask;
    private float rainYOffset = -1903f; //Max accessable height. Change after map designed.
    private bool IsRainActive;
    private Transform rainTransform;
    private PlayerController playerController;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        IsRainActive = false;
        rainTransform = rainPS.transform;
        bitMask = groundLayer | waterLayer;
    }

    private void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void Update()
    {
        if (IsRainActive)
        {
            AudioManager.PlayRainAudio();
            AudioManager.PlayWindAudio();
        }
        else
        {
            AudioManager.StopRainAudio();
            AudioManager.StopWindAudio();
        }       
    }

    private void LateUpdate()
    {
        Vector3 pos = playerController.transform.position;
        transform.position = new Vector3(pos.x + Xoffset, transform.position.y, pos.z + Zoffset);
        //This method control rain with respect to height
        //CheckWithGround();
        //void CheckWithGround()
        //{
        //    if (Physics.Raycast(rainTransform.position, Vector3.down, out RaycastHit hit, 300, bitMask))
        //    {
        //        float dif = rainTransform.localPosition.y - hit.point.y;
        //        float adjust = 0;
        //        Debug.Log(dif);
        //        if (dif > rainYOffset + 5)
        //        {
        //            adjust = dif - rainYOffset;
        //        }
        //        if (dif < rainYOffset - 5)
        //        {
        //            adjust = rainYOffset - dif;
        //        }
        //        rainTransform.localPosition = new Vector3(0, rainTransform.localPosition.y + adjust, 0);
        //    }
        //}
    }

    #endregion

    #region Mechanics

    public void ControlRain()
    {
        if (!IsRainActive)
        {            
            rainPS.Play();
            lightningFlash.SetActive(true);            
            IsRainActive = true;
            
        }
        else
        {            
            rainPS.Stop();
            lightningFlash.SetActive(false);           
            IsRainActive = false;
        }       
    }

    #endregion

}
