using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDecalLauncher : MonoBehaviour
{
    #region Variables

    [SerializeField] private Gradient decalColorGradient;
    private ParticleSystem particleLauncher;
    private BloodDecalPool decalPool;
    private List<ParticleCollisionEvent> collisionEvents;

    #endregion

    #region General

    private void Awake()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        particleLauncher = GetComponent<ParticleSystem>();
        decalPool = GetComponentInChildren<BloodDecalPool>();
    }   

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);
        for (int i = 0; i < collisionEvents.Count; i++)
        {
            decalPool.ParticleHit(collisionEvents[i], decalColorGradient);
        }        
    }

    #endregion
}
