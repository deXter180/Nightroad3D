using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSystem : PersistentSingleton<RainSystem>
{
    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private GameObject lightningFlash;
    [SerializeField] private float Xoffset = 0;
    [SerializeField] private float Zoffset = -10;
    private int groundLayer = 1 << 8;
    private int waterLayer = 1 << 4;
    private int bitMask;
    private float rainYOffset = -1903f;
    private bool IsRainActive;
    private AudioManager audioManager;
    private Transform rainTransform;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        IsRainActive = false;
        rainTransform = rainPS.transform;
        bitMask = groundLayer | waterLayer;
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        playerController = PlayerController.Instance;
    }

    private void Update()
    {
        if (IsRainActive)
        {
            audioManager.PlayRainAudio();
            audioManager.PlayWindAudio();
        }
        else
        {
            audioManager.StopRainAudio();
            audioManager.StopWindAudio();
        }       
    }

    private void LateUpdate()
    {
        Vector3 pos = playerController.transform.position;
        transform.position = new Vector3(pos.x + Xoffset, transform.position.y, pos.z + Zoffset);
        CheckWithGround();
        void CheckWithGround()
        {
            if (Physics.Raycast(rainTransform.position, Vector3.down, out RaycastHit hit, 300, bitMask))
            {
                float dif = rainTransform.localPosition.y - hit.point.y;
                float adjust = 0;
                if (dif > rainYOffset + 5)
                {
                    adjust = dif - rainYOffset;
                }
                if (dif < rainYOffset - 5)
                {
                    adjust = rainYOffset - dif;
                }
                rainTransform.localPosition = new Vector3(0, rainTransform.localPosition.y + adjust, 0);
            }
        }
    }

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
   
}
