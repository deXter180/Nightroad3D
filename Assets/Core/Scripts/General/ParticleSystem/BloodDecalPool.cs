using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDecalPool : MonoBehaviour
{
    [SerializeField] private int maxDecals = 100;
    [SerializeField] private float decalSizeMin = 0.5f;
    [SerializeField] private float decalSizeMax = 1.5f;
    private int decalDataIndex;
    private ParticleSystem decalParticleSystem;
    private BloodDecalData[] decalData;
    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        decalDataIndex = 0;
        decalParticleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[maxDecals];
        decalData = new BloodDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++)
        {
            decalData[i] = new BloodDecalData();
        }
    }

    private void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        if (decalDataIndex >= maxDecals)
        {
            decalDataIndex = 0;
        }
        decalData[decalDataIndex].position = particleCollisionEvent.intersection;
        Vector3 rotationEuler = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
        rotationEuler.x = Random.Range(0, 360);
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
        }
        decalParticleSystem.SetParticles(particles, particles.Length);
    }

    public void ParticleHit (ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        SetParticleData(particleCollisionEvent, colorGradient);
        DisplayParticle();
    }


}
