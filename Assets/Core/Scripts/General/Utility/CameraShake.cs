using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Shakes the camera in an organic way, based on Perlin noise.
/// Supports any initial camera position and rotation, but camera should be steady, i.e. parented to another GameObject.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraShake : Singleton<CameraShake>
{
    private ShakeProperty currentProperty;
    public bool DeltaMovement = true;
    protected Camera Camera;
    protected float time = 0;
    protected Vector3 lastPos;
    protected Vector3 nextPos;
    protected float lastFoV;
    protected float nextFoV;
    protected bool destroyAfterPlay;

    protected override void Awake()
    {
        base.Awake();
        Camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (time > 0 && currentProperty != null)
        {
            time -= Time.deltaTime;
            if (time > 0)
            {
                nextPos = (Mathf.PerlinNoise(time * currentProperty.Speed, time * currentProperty.Speed * 2) - 0.5f) * currentProperty.Amount.x * transform.right * currentProperty.Curve.Evaluate(1f - time / currentProperty.Duration) +
                          (Mathf.PerlinNoise(time * currentProperty.Speed * 2, time * currentProperty.Speed) - 0.5f) * currentProperty.Amount.y * transform.up * currentProperty.Curve.Evaluate(1f - time / currentProperty.Duration);
                nextFoV = (Mathf.PerlinNoise(time * currentProperty.Speed * 2, time * currentProperty.Speed * 2) - 0.5f) * currentProperty.Amount.z * currentProperty.Curve.Evaluate(1f - time / currentProperty.Duration);

                Camera.fieldOfView += (nextFoV - lastFoV);
                Camera.transform.Translate(DeltaMovement ? (nextPos - lastPos) : nextPos);

                lastPos = nextPos;
                lastFoV = nextFoV;
            }
            else
            {
                ResetCam();
                if (destroyAfterPlay)
                {
                    time = 0;
                    currentProperty = null;
                }
            }
        }
    }

    public void ShakeOnce(ShakeProperty property)
    {
        float currentTime = 0;
        float actualFoView = Camera.fieldOfView;
        float lastFoView = 0;
        float nextFoView = 0;
        Vector3 lastPosition = Vector3.zero;
        Vector3 nextPosition = Vector3.zero;
        while (currentTime <= property.Duration)
        {          
            nextPosition = (Mathf.PerlinNoise(Time.fixedDeltaTime * property.Speed, Time.fixedDeltaTime * property.Speed * 2) - 0.5f) * property.Amount.x * transform.right * property.Curve.Evaluate(1f - Time.fixedDeltaTime / property.Duration) +
                              (Mathf.PerlinNoise(Time.fixedDeltaTime * property.Speed * 2, Time.fixedDeltaTime * property.Speed) - 0.5f) * property.Amount.y * transform.up * property.Curve.Evaluate(1f - Time.fixedDeltaTime / property.Duration);
            nextFoView = (Mathf.PerlinNoise(Time.fixedDeltaTime * property.Speed * 2, Time.fixedDeltaTime * property.Speed * 2) - 0.5f) * property.Amount.z * property.Curve.Evaluate(1f - Time.fixedDeltaTime / property.Duration);

            Camera.fieldOfView += (nextFoView - lastFoView);
            Camera.transform.Translate(nextPosition - lastPosition);
            lastPosition = nextPosition;
            lastFoView = nextFoView;
            currentTime += Time.fixedDeltaTime;
        }
        Camera.transform.Translate(Vector3.zero);
        Camera.fieldOfView = actualFoView;

    }

    public void StartShake(ShakeProperty property, bool deltaMovement = true)
    {
        currentProperty = property;
        DeltaMovement = deltaMovement;
        destroyAfterPlay = true;
        Shake();
    }

    public void StopShake()
    {
        if (currentProperty != null)
        {
            ResetCam();
            time = 0;
            currentProperty = null;
        }
    }

    private void Shake()
    {
        if (currentProperty != null)
        {
            ResetCam();
            time = currentProperty.Duration;
        }        
    }

    private void ResetCam()
    {
        Camera.transform.Translate(DeltaMovement ? -lastPos : Vector3.zero);
        Camera.fieldOfView -= lastFoV;
        lastPos = nextPos = Vector3.zero;
        lastFoV = nextFoV = 0f;
    }

    [System.Serializable]
    public class ShakeProperty
    {
        [Tooltip("Amount of shake in each axis")] public Vector3 Amount;
        [Tooltip("Duration of shake")] public float Duration;
        [Tooltip("Speed of shake")] public float Speed;
        [Tooltip("Curve to control shake intensity over time")] public AnimationCurve Curve;
    }
}


