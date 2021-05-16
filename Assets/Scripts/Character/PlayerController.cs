using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~

    public static PlayerController Instance { get; private set; }
    [SerializeField] private int MaxHP;
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
    private Rigidbody RB;
    private Collider col;
    private Input input;

    //~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        input = GetComponent<InputControl>();
    }

    private void OnEnable()
    {
        RB.useGravity = false;
    }

    private void OnDisable()
    {
        
    }

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        SetGravity();
        Rotate();
        Move();
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
        float moveX = input.GetMovement().x;
        float moveY = input.GetMovement().y;
        Vector3 move = transform.right * moveX + transform.forward * moveY;
        RB.MovePosition(transform.position + move * MoveSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        float horizontalLook = input.GetMousePos().x * MouseSensitivity * Time.fixedDeltaTime;
        float verticalLook = input.GetMousePos().y * MouseSensitivity * Time.fixedDeltaTime;
        camControl -= verticalLook;
        camControl = Mathf.Clamp(camControl, -90f, 90f);
        cam.localEulerAngles = new Vector3(camControl, 0f, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x , transform.rotation.eulerAngles.y + horizontalLook, 0);
    }

    private void Jump()
    {
        if (input.GetJump() == 1)
        {
            if (GroundCheck())
            {
                RB.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            }
        }
    }
    private bool GroundCheck()
    {
        RaycastHit hit;
        Physics.BoxCast(col.bounds.center, transform.localScale, Vector3.down, out hit, Quaternion.identity, col.bounds.extents.y + 0.01f, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}