using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    [SerializeField] private ImpactTypes impactType;
    public ImpactTypes ImpactType { get => impactType; }
}
