using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToAngle : MonoBehaviour
{
    private Vector3 _targetDir;
    private float _angle;
    private int _lastIndex;
    private bool _readyForAnim = false;
    public int LastIndex => _lastIndex;
    public bool ReadyForAnim => _readyForAnim;
    public float Angle => _angle;

    private void Start()
    {
        _readyForAnim = true;
    }

    public void CalculateAngle(Vector3 _targetPos)
    {
        _targetDir = _targetPos - transform.position;
        _angle = Vector3.SignedAngle(_targetDir, transform.forward, Vector3.up);
        _lastIndex = GetIndex(_angle);
    }

    private int GetIndex(float _angl)
    {
        //front
        if (_angl <= -133 || _angl >= 133)
        {
            _lastIndex = 0;
        }

        //Left
        else if (_angl >= -133 && _angl < -43)
        {
            _lastIndex = 3;
        }

        //back
        else if (_angl >= -43 && _angl <= 43)
        {
            _lastIndex = 2;
        }

        //Right
        else if (_angl > 43 && _angl <= 133)
        {
            _lastIndex = 1;
        }
              
        return _lastIndex;
    }

}
