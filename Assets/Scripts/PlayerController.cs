using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour, InputControls.IGameplayActions
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~

    [SerializeField] private int MaxHP;
    [SerializeField] private InputControls Controls;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float JumpForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cam;
    private static float globalGravity = -9.81f;
    private float camControl;
    private Vector2 _movement;
    private Vector2 _look;
    private Rigidbody RB;
    private Collider col;
    private Vector3 Direction;
    private bool IsGrounded;
    private bool IsJump;

    //~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        Controls = new InputControls();
        Controls.Gameplay.SetCallbacks(this);  
    }

    private void OnEnable()
    {
        Controls.Enable();
        RB.useGravity = false;
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    //~~~~~~~~~~~~~~~~ Input callbacks ~~~~~~~~~~~~~~~~~~~~

    public void OnMovement(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
        Vector3 _horizontal = transform.right * _movement.x;
        Vector3 _vertical = transform.forward * _movement.y;
        Direction = _horizontal + _vertical;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (context.performed)
            {
                IsJump = context.ReadValueAsButton();
            }
        }
        else IsJump = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        SetGravity();
        Move();
        Rotate();
        Jump();
    }

    private void Update()
    {
        //OnTakingDamage()
    }

    private void SetGravity()
    {
        Vector3 gravity = globalGravity * GravityScale * Vector3.up;
        RB.AddForce(gravity, ForceMode.Impulse);
    }

    private void Move()
    {
        RB.MovePosition(transform.position + (Direction * MoveSpeed * Time.fixedDeltaTime));
    }

    private void Rotate()
    {
        float _horizontalLook = _look.x * MouseSensitivity * Time.fixedDeltaTime;
        float _verticalLook = _look.y * MouseSensitivity * Time.fixedDeltaTime;
        camControl -= _verticalLook;
        camControl = Mathf.Clamp(camControl, -90f, 90f);
        cam.localEulerAngles = new Vector3(camControl, 0f, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _horizontalLook, 0);
    }

    private void Jump()
    {
        RaycastHit hit;
        Physics.BoxCast(col.bounds.center, transform.localScale, Vector3.down, out hit, Quaternion.identity, col.bounds.extents.y + 0.01f, groundLayer) ;
        if (hit.collider != null)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
        if(IsGrounded && IsJump)
        {
            RB.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

}




