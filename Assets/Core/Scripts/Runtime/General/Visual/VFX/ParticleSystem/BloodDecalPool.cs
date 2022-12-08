using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDecalPool : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private int maxDecals = 100;
    [SerializeField] private float decalSizeMin = 0.5f;
    [SerializeField] private float decalSizeMax = 1.5f;
    [SerializeField] private float decalLifetimeMax = 5;
    [SerializeField] private float decalLifetimeMin = 2;

    #endregion

    #region Variables

    private ParticleSystem decalParticleSystem;
    private int decalDataIndex;
    private BloodDecalData[] decalData;
    private ParticleSystem.Particle[] particles;

    #endregion

    #region General

    private void Start()
    {
        decalParticleSystem = GetComponent<ParticleSystem>();
        decalDataIndex = 0;
        particles = new ParticleSystem.Particle[maxDecals];
        decalData = new BloodDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++)
        {
            decalData[i] = new BloodDecalData();
        }
    }

    #endregion

    #region Mechanics

    public void ResetParticleDataIndex()
    {
        decalDataIndex = 0;
    }

    private void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {       
        if (decalDataIndex >= maxDecals)
        {
            decalDataIndex = 0;
        }
        decalData[decalDataIndex].position = particleCollisionEvent.intersection + new Vector3(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f), 0);
        Vector3 rotationEuler = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
        rotationEuler.z = Random.Range(0, 360);
        decalData[decalDataIndex].rotation = rotationEuler;
        decalData[decalDataIndex].size = Random.Range(decalSizeMin, decalSizeMax);
        decalData[decalDataIndex].color = colorGradient.Evaluate(Random.Range(0f, 1f));
        decalDataIndex++;
    }

    private void DisplayParticle()
    {
        for (int i = 0; i < decalData.Length; i++)
        {
            particles[i].position = decalData[i].position;
            particles[i].rotation3D = decalData[i].rotation;
            particles[i].startSize = decalData[i].size;
            particles[i].startColor = decalData[i].color;
            var time = Random.Range(decalLifetimeMin, decalLifetimeMax);
            particles[i].startLifetime = time;
            particles[i].remainingLifetime = time;
        }
        decalParticleSystem.SetParticles(particles, particles.Length);
    }

    public void ParticleHit (ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        SetParticleData(particleCollisionEvent, colorGradient);
        DisplayParticle();
    }

    #endregion
}
