using System;
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
    public float GroundHeight;
    [SerializeField] private int HitPoints;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float JumpForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackForce;
    [SerializeField] private float dodgeChance;
    [SerializeField] private float SelectionThreshold;
    [SerializeField] private Transform camTransform;
    private PickedObject selectedPickedObject = null;
    private static float globalGravity = -9.81f;
    private Rigidbody RB;
    private CapsuleCollider col;
    private Target target;
    private Vector3 OldPos;
    private Vector3 gravity;
    private Input input;
    private int ground = 1 << 8;
    private int water = 1 << 4;
    private int pickableLayer = 1 << 6;
    private int bitmask;
    private float camControlX = 0f;
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
        col = GetComponent<CapsuleCollider>();
        input = GetComponent<InputControl>();
        target = GetComponent<Target>();
        bitmask = ground | water;
        gravity = globalGravity * GravityScale * Vector3.up;
        
    }

    private void OnEnable()
    {
        RB.useGravity = false;
        GetComponent<Target>().SetupMaxHP(MaxHP);
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
        RB.AddForce(gravity, ForceMode.Impulse);
    }

    private void Move()
    {
        float moveX = input.GetMovement().x;
        float moveY = input.GetMovement().y;
        Vector3 movePos = transform.position + (transform.right * moveX + transform.forward * moveY) * MoveSpeed * Time.fixedDeltaTime;
        OldPos = transform.position;
        RB.MovePosition(movePos);
    }

    private void Rotate()
    {
        float horizontalLook = input.GetMouseDelta().x * MouseSensitivity * Time.fixedDeltaTime;
        float verticalLook = input.GetMouseDelta().y * MouseSensitivity * Time.fixedDeltaTime;
        camControlX -= verticalLook;
        camControlX = Mathf.Clamp(camControlX, -90f, 90f);
        camTransform.localEulerAngles = new Vector3(camControlX, transform.rotation.eulerAngles.y + horizontalLook, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + horizontalLook, 0);
    }

    //private void RotateChild() ** Don't use as a weird bug occuring with weapon visual rotating **
    //{
    //    float horizontalLook = input.GetMouseDelta().x * MouseSensitivity * Time.fixedDeltaTime;
    //    float verticalLook = input.GetMouseDelta().y * MouseSensitivity * Time.fixedDeltaTime;
    //    float camControlY = 0f;
    //    camControlX -= verticalLook;
    //    camControlY += horizontalLook;
    //    camControlX = Mathf.Clamp(camControlX, -90f, 90f);
    //    camTransform.localEulerAngles = new Vector3(camControlX, camControlY, 0f);        
    //    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + horizontalLook, 0);
    //}

    private void Jump()
    {
        if (input.GetJump() == 1)
        {
            if (GroundCheck())
            {
                if (RB.velocity.y > 2f)
                    RB.AddForce(transform.up * (JumpForce / 2), ForceMode.Impulse);
                else
                    RB.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            }
        }
    }

    private bool GroundCheck()
    {
        Physics.BoxCast(col.bounds.center, transform.localScale / 2, Vector3.down, out RaycastHit hit, Quaternion.identity, col.bounds.extents.y, bitmask);
        return hit.collider != null;
    }

    public bool IsMoving(Vector3 newPos)
    {
        if (newPos == OldPos)
        {
            return false;
        }
        else return true;
    }

    public Vector3 GetRandomDirWithoutY(float minRange, float maxRange)
    {
        return new Vector3(UnityEngine.Random.Range(minRange, maxRange), 0, UnityEngine.Random.Range(minRange, maxRange)).normalized;
    }

    public Vector3 GetRandomPosWithoutY(float minRange, float maxRange)
    {
        return new Vector3(UnityEngine.Random.Range(minRange, maxRange), 0, UnityEngine.Random.Range(minRange, maxRange));
    }

    public Vector3 GetRandomPosWithConstY(float minRange, float maxRange, float Y)
    {
        return new Vector3(UnityEngine.Random.Range(minRange, maxRange), Y, UnityEngine.Random.Range(minRange, maxRange));
    }

    public Vector3 GetRandomPosWithVariableY(float minRange, float maxRange)
    {
        return new Vector3(UnityEngine.Random.Range(minRange, maxRange), UnityEngine.Random.Range(minRange, maxRange), UnityEngine.Random.Range(minRange, maxRange));
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
                        selectedPickedObject.AddForceToItemSpawn(GetRandomDirWithoutY(1f, -1f));
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