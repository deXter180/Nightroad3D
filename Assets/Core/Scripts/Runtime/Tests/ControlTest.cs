using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTest : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform camTransform;
    private float camControlX = 0f;
    private PlayerInputAsset inputs;
    private bool isMoving;
    private bool isReset;
    private float MouseSensitivity = 20f;
    private float MoveSpeed = 7;
    private float SlantingSpeed = 0.15f;
    private float SlantingPower = 4;
    private float elapseTime = 0;
    private float currentTime = 0;
    private Vector2 movePosition = Vector2.zero;
    private Vector3 dashPos;
    private Quaternion oldRot;
    private Rigidbody RB;

    #endregion

    #region General

    private void Awake()
    {
        RB = GetComponent<Rigidbody>(); 
        if (inputs == null)
        {
            inputs = new PlayerInputAsset();
            inputs.Enable();
        }
    }

    private void Start()
    {
        isMoving = false;
        isReset = false;
        oldRot = Quaternion.identity;
    }

    private void Update()
    {
        if (inputs != null)
        {
            HandleBaseMechanics();
        }
    }

    #endregion

    #region Mechanics

    private void HandleBaseMechanics()
    {
        while (currentTime < Time.time)
        {
            Move();
            Rotate();
            if (RB.velocity == Vector3.zero) movePosition = Vector2.zero;
            currentTime += Time.fixedDeltaTime;
        }
    }

    private void Rotate()
    {
        Vector2 mouseDeltaPos = inputs.BasicControls.MouseDelta.ReadValue<Vector2>();
        float horizontalLook = mouseDeltaPos.x * MouseSensitivity * Time.fixedDeltaTime;
        float verticalLook = mouseDeltaPos.y * MouseSensitivity * Time.fixedDeltaTime;
        camControlX -= verticalLook;
        camControlX = Mathf.Clamp(camControlX, -90f, 90f);
        float rotY = transform.rotation.eulerAngles.y + horizontalLook;
        if (!isMoving)
        {
            if (oldRot != Quaternion.identity)
            {
                elapseTime += Time.fixedDeltaTime;
                if (elapseTime > SlantingSpeed)
                {
                    elapseTime = SlantingSpeed;
                    isMoving = false;
                }
                float perc = elapseTime / SlantingSpeed;
                oldRot.eulerAngles = new Vector3(camControlX, camTransform.rotation.eulerAngles.y, oldRot.eulerAngles.z);
                camTransform.rotation = Quaternion.Slerp(camTransform.rotation, oldRot, perc);
                if (perc == 1)
                {
                    oldRot = Quaternion.identity;
                    isReset = false;
                }
            }
            else
            {
                camTransform.eulerAngles = new Vector3(camControlX, rotY, 0f);
            }
        }
        else
        {
            if (!isReset)
            {
                oldRot = camTransform.rotation;
            }
            elapseTime = 0;
            float rotX;
            if (camControlX < -87 || camControlX > 87)
            {
                rotX = camControlX;
            }
            else
            {
                rotX = Mathf.LerpAngle(camTransform.eulerAngles.x, camControlX + (movePosition.y * SlantingPower / 2), Time.fixedDeltaTime / SlantingSpeed);
            }
            float rotZ = Mathf.LerpAngle(camTransform.eulerAngles.z, -movePosition.x * SlantingPower, Time.fixedDeltaTime / SlantingSpeed);
            camTransform.eulerAngles = new Vector3(rotX, rotY, rotZ);
            isReset = true;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + horizontalLook, 0);
    }

    private void Move()
    {
        Vector3 wishDir;
        movePosition = inputs.BasicControls.Movement.ReadValue<Vector2>();
        if (movePosition != Vector2.zero)
        {
            float moveX = movePosition.x;
            float moveY = movePosition.y;
            wishDir = new Vector3(moveX, 0, moveY);
            wishDir = transform.TransformDirection(wishDir);
            wishDir.Normalize();
            var wishSpeed = wishDir.magnitude;
            wishSpeed *= MoveSpeed;        
            dashPos = transform.right * moveX + transform.forward * moveY;
            Vector3 movePos = Vector3.zero;
            movePos = transform.position + dashPos * Time.fixedDeltaTime * MoveSpeed;
            RB.MovePosition(movePos);
            isMoving = true;
        }
    }

    #endregion
}
