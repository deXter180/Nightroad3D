using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~
    
    public float GroundHeight;
    public static PlayerController Instance { get; private set; }
    public Target PlayerTarget { get => target; }
    public int MaxHP { get => HitPoints; }
    public float DodgeChace { get => dodgeChance; }  
    public bool IsCrouching { get => isCrouching; }
    public bool IsPlayerDead { get => isDead; }
    public Vector3 DashPos { get => dashPos; }
    public Rigidbody PlayerRB { get => RB; }
    [SerializeField] private int HitPoints;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float JumpForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackForce;
    [SerializeField] private float dodgeChance;
    [SerializeField] private float crouchHeightPrecentage;
    [SerializeField] private float SelectionThreshold;
    [SerializeField] private Transform camTransform;
    [SerializeField] private InventorySystem mainInventory;
    private PlayerInputAsset inputs;
    private PickedObject selectedPickedObject = null;
    private static float globalGravity = -9.81f;
    private Rigidbody RB;
    private CapsuleCollider col;
    private Target target;
    private Vector3 OldPos;
    private Vector3 gravity;
    private Vector3 dashPos;
    //private Input input;
    private int ground = 1 << 8;
    private int water = 1 << 4;
    private int pickableLayer = 1 << 6;
    private int bitmask;
    private float camControlX = 0f;
    private float normalHeight;
    private float crouchHeight;  
    private float currentTime = 0;
    private bool isCrouching;
    private bool isDead;
    private bool isToggledCrouching;
    public static event Action<InventoryItemSO> OnItemPicked;
    public static event Action OnPlayerDeath;

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
        target = GetComponent<Target>();
        bitmask = ground | water;
        gravity = globalGravity * GravityScale * Vector3.up;
        StartCoroutine(mainInventory.Test());
        isDead = false;
    }

    private void OnEnable()
    {
        StartCoroutine(InputDone());
        RB.useGravity = false;
        GetComponent<Target>().SetupMaxHP(MaxHP);
        normalHeight = col.height;
        crouchHeight = (normalHeight * crouchHeightPrecentage) / 100;
        //PickedObjectCols = new List<Collider>();
    }

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        SetGravity();                 
    }

    private void Update()
    {
        if (!target.IsDead)
        {
            if (inputs != null)
            {
                //OnTakingDamage()
                InteractInWorld();
                HandleBaseMechanics();
            }
        }
        else
        {
            if (!isDead)
            {
                OnPlayerDeath?.Invoke();
                isDead = true;
            }            
        }              
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
    }

    private void HandleBaseMechanics()
    {
        while (currentTime < Time.time)
        {
            if (!InventoryUIHandler.Instance.IsInventoryActive && !InputMenuUIHandler.Instance.IsMainMenuActive)
            {
                Rotate();
                Move(MoveSpeed);
                Jump();
                HoldCrouch();
                ToggleCrouch();
            }
            currentTime += Time.fixedDeltaTime;
        }
    }

    private void SetGravity()
    {
        RB.AddForce(gravity, ForceMode.Impulse);
    }

    private void Move(float moveSpeed)
    {
        Vector2 movePosition = inputs.BasicControls.Movement.ReadValue<Vector2>();
        float moveX = movePosition.x;
        float moveY = movePosition.y;
        dashPos = transform.right * moveX + transform.forward * moveY;
        Vector3 movePos = transform.position + (dashPos * moveSpeed * Time.fixedDeltaTime);
        OldPos = transform.position;
        RB.MovePosition(movePos);
    }

    private void Rotate()
    {
        Vector2 mouseDeltaPos = inputs.BasicControls.MouseDelta.ReadValue<Vector2>();
        float horizontalLook = mouseDeltaPos.x * MouseSensitivity * Time.fixedDeltaTime;
        float verticalLook = mouseDeltaPos.y * MouseSensitivity * Time.fixedDeltaTime;
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
        if (inputs.BasicControls.Jump.ReadValue<float>() == 1)
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

    private void HoldCrouch()
    {
        if (GroundCheck() && !isToggledCrouching)
        {
            if (inputs.BasicControls.HoldCrouch.ReadValue<float>() == 1)
            {
                if (!isCrouching)
                {
                    isCrouching = true;
                    col.height = crouchHeight;
                }
            }
            else
            {
                if (isCrouching)
                {
                    isCrouching = false;
                    col.height = normalHeight;
                }
                
            }
        }      
    }

    private void ToggleCrouch()
    {
        if (GroundCheck())
        {
            if (inputs.BasicControls.ToggleCrouch.triggered)
            {
                if (!isCrouching)
                {
                    isCrouching = true;
                    isToggledCrouching = true;
                    col.height = crouchHeight;
                }
                else
                {
                    isCrouching = false;
                    isToggledCrouching = false;
                    col.height = normalHeight;
                }
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

    private void InteractInWorld()
    {
        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(input.GetMousePosition());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane / 2, pickableLayer))
        {
            if (hit.transform.TryGetComponent<PickedObject>(out selectedPickedObject))
            {
                selectedPickedObject.HighlightObject();
                if (inputs.BasicControls.Interact.triggered)//input.GetPickItems())
                {
                    if (mainInventory.TryAddingItem(selectedPickedObject.GetItemSO()))
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

    public float GetAngle(GameObject _target)
    {
        Quaternion _rotationA = transform.rotation;
        Quaternion _rotationB = _target.transform.rotation;

        float _angle = Quaternion.Angle(_rotationA, _rotationB);

        Vector3 _forwardA = _rotationA * Vector3.forward;
        Vector3 _forwardB = _rotationB * Vector3.forward;

        float _angleA = Mathf.Atan2(_forwardA.x, _forwardA.z) * Mathf.Rad2Deg;
        float _angleB = Mathf.Atan2(_forwardB.x, _forwardB.z) * Mathf.Rad2Deg;

        return Mathf.DeltaAngle(_angleA, _angleB);
    }

    //~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~
}