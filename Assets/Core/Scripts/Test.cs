using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform slashPrefab;
    [SerializeField] [Range(0, 360)] [Tooltip("In Degrees")] private float spreadAngle = 0;
    private float elapsedTime;
    private bool isActivate;

    private void Start()
    {
        elapsedTime = 0;
        isActivate = false;       
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 5 && !isActivate)
        {
            Transform first = Instantiate(slashPrefab, transform);
            Transform second = Instantiate(slashPrefab, transform);
            Transform third = Instantiate(slashPrefab, transform);
            first.forward = transform.forward;
            second.forward = Helpers.DirFromAngle(transform, -spreadAngle / 2, false);
            third.forward = Helpers.DirFromAngle(transform, spreadAngle / 2, false);
            isActivate = true;
        }
    }
}
