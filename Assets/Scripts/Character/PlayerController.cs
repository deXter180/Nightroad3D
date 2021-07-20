﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~

    public static PlayerController Instance { get; private set; }
    public Target PlayerTarget { get => target; }
    public int MaxHP { get => HitPoints; }
    public float DodgeChace { get => dodgeChance; }
    [SerializeField] private int HitPoints;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float JumpForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackForce;
    [SerializeField] private float dodgeChance;
    [SerializeField] private float SelectionThreshold;
    [SerializeField] private Transform cam;
    //private List<Collider> PickedObjectCols;
    private PickedObject selectedPickedObject = null;
    private static float globalGravity = -9.81f;
    private float groundHeight;
    private float camControl;
    private Rigidbody RB;
    private Collider col;
    private Target target;
    private Vector3 OldPos;
    private Input input;
    private int ground = 1 << 8;
    private int water = 1 << 4;
    private int pickableLayer = 1 << 6;
    private int bitmask;    
    public static event Action<InventoryItemSO> OnItemPicked;

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
        target = GetComponent<Target>();
        bitmask = ground | water;
    }

    private void OnEnable()
    {
        RB.useGravity = false;
        //PickedObjectCols = new List<Collider>();
    }

    private void OnDisable()
    {
        
    }

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        SetGravity(); 
        if (!InventoryUIHandler.Instance.IsInventoryON)
        {
            Rotate();
            Move();
            Jump();
        }        
    }

    private void Update()
    {
        //OnTakingDamage()
        ControlInventory();
        SearchItemsInWorld();
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
        OldPos = transform.position;
        RB.MovePosition(transform.position + move * MoveSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        float horizontalLook = input.GetMouseDelta().x * MouseSensitivity * Time.fixedDeltaTime;
        float verticalLook = input.GetMouseDelta().y * MouseSensitivity * Time.fixedDeltaTime;
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
                Debug.Log(GetGroundHeight());
            }
        }
    }

    private bool GroundCheck()
    {
        Physics.BoxCast(col.bounds.center, transform.localScale, Vector3.down, out RaycastHit hit, Quaternion.identity, col.bounds.extents.y + 15f, bitmask);
        if (hit.collider != null)
        {
            groundHeight = hit.transform.position.y;
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetGroundHeight()
    {
        GroundCheck();
        return groundHeight;
    }

    public bool IsMoving()
    {
        if (RB.position == OldPos)
        {
            return false;
        }
        else return true;
    }

    public Vector3 GetRandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private void ControlInventory()
    {
        if (input.GetInventory())
        {
            InventoryUIHandler.Instance.Control(); 
            if (InventoryUIHandler.Instance.IsInventoryON)
            {
                InventoryUIHandler.Instance.IsInventoryON = false;
            }
            else InventoryUIHandler.Instance.IsInventoryON = true;
        }
    }

    private void SearchItemsInWorld()
    {
        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(input.GetMousePosition());
        Debug.DrawRay(ray.origin, ray.direction * Camera.main.farClipPlane / 2);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane / 2, pickableLayer))
        {
            selectedPickedObject = hit.transform.GetComponent<PickedObject>();
            if (selectedPickedObject != null)
            {
                selectedPickedObject.HighlightObject();
                if (input.GetPickItems())
                {
                    if (InventorySystem.Instance.TryAddingItem(selectedPickedObject.GetItemSO()))
                    {
                        selectedPickedObject.DestroySelf();
                        OnItemPicked?.Invoke(selectedPickedObject.GetItemSO());
                    }
                    else
                    {
                        selectedPickedObject.AddForceToItemSpawn(GetRandomDirection());
                    }
                }
            }
        }
        else
        {
            if (selectedPickedObject != null)
            {
                selectedPickedObject.UnhighlightObject();
                selectedPickedObject = null;
            }
        }
    }
}