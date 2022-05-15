using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : PersistentSingleton<PlayerController>
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~
    
    public float GroundHeight;
    public Target PlayerTarget { get => target; }
    public int MaxHitPoints { get => maxHitPoints; }
    public int MaxMana { get => maxMana; }
    public int CurrentHP { get; private set; }
    public int CurrentMana { get; private set; }
    public float DodgeChace { get => dodgeChance; }  
    public bool IsCrouching { get => isCrouching; }
    public bool IsPlayerDead { get => isDead; }
    public Vector3 DashPos { get => dashPos; }
    public Rigidbody PlayerRB { get => RB; }
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int maxMana;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float JumpForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackForce;
    [SerializeField] private float dodgeChance;
    [SerializeField] private float crouchHeightPrecentage;
    [SerializeField] private float SelectionThreshold;
    [SerializeField] private float SlantingPower;
    [SerializeField] private float SlantingSpeed;
    private Transform camTransform;
    private InventorySystem mainInventory;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private WeaponManager weaponManager;
    private SpellManager spellManager;
    private DialogueManager dialogueManager;
    private PickedObject selectedPickedObject = null;
    private NPCBrain selectedNPC = null;
    private static float globalGravity = -9.81f;
    private Rigidbody RB;
    private CapsuleCollider col;
    private Target target;
    private AudioListener audioListener;
    private Vector2 movePosition;
    private Vector3 OldPos;
    private Vector3 gravity;
    private Vector3 dashPos;
    private Vector3 originalPlayerPos;
    private Vector3 ConstantDistFromPlayer;
    //private Input input;
    private int ground = 1 << 8;
    private int water = 1 << 4;
    private int pickableLayer = 1 << 6;
    private int npcLayer = 1 << 14;
    private int bitmask;
    private float camControlX = 0f;
    private float normalHeight;
    private float crouchHeight;  
    private float currentTime = 0;
    private float gravityControl;
    private bool isJumping;
    private bool isCrouching;
    private bool isDead;
    private bool isToggledCrouching;  
    public static event Action OnPlayerDeath;

    //~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~

    protected override void Awake()
    {
        base.Awake();
        RB = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();       
        target = GetComponent<Target>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        spellManager = GetComponentInChildren<SpellManager>();
        bitmask = ground | water;
        
        isDead = false;
    }

    private void Start()
    {
        camTransform = Helpers.MainCam.transform;
        ConstantDistFromPlayer = camTransform.position - transform.position;
        StartCoroutine(InputDone());
        AssignInv();
        AssignGameControl();
        AssignDialogueManager();
        audioListener = Helpers.MainCam.GetComponent<AudioListener>();
        RB.useGravity = false;
        normalHeight = col.height;
        crouchHeight = (normalHeight * crouchHeightPrecentage) / 100;
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        target.Resource.OnHealthGain += Resource_OnHealthGain;
        target.Resource.OnHealthLoss += Resource_OnHealthLoss;
        target.Resource.OnManaGain += Resource_OnManaGain;
        target.Resource.OnManaLoss += Resource_OnManaLoss;
        isJumping = false;
        originalPlayerPos = transform.position;
        //PickedObjectCols = new List<Collider>();
    }

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        if (RB != null)
        {
            if (RB.velocity.y > 14)
            {
                gravityControl = GravityScale + 1;
            }
            else
            {
                gravityControl = GravityScale;
            }
            SetGravity();           
        }                        
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
                PostPlayerDeath();
            }            
        }              
    }

    private void PostPlayerDeath()
    {
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        target.Resource.OnHealthGain -= Resource_OnHealthGain;
        target.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        target.Resource.OnManaGain -= Resource_OnManaGain;
        target.Resource.OnManaLoss -= Resource_OnManaLoss;
    }

    private IEnumerator InputDone()
    {
        if (inputs == null)
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
        }      
    }

    private void HandleBaseMechanics()
    {
        while (currentTime < Time.time)
        {
            if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsDialogueActive)
            {                                              
                Move(MoveSpeed);
                Rotate();
                UpdateCameraPosition();
                Jump();
                HoldCrouch();
                ToggleCrouch();
                if (RB.velocity == Vector3.zero) movePosition = Vector2.zero;
            }
            currentTime += Time.fixedDeltaTime;
        }
    }

    private void AssignInv()
    {
        if (mainInventory == null)
            mainInventory = InventorySystem.Instance;
    }

    private void AssignGameControl()
    {
        if (gameController == null)
            gameController = GameController.Instance;
    }

    private void AssignDialogueManager()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
    }

    private void SetGravity()
    {
        gravity = globalGravity * gravityControl * Vector3.up;
        RB.AddForce(gravity, ForceMode.Impulse);
    }

    private void UpdateCameraPosition()
    {
        camTransform.position = transform.position + ConstantDistFromPlayer;
    }

    private void Move(float moveSpeed)
    {
        movePosition = inputs.BasicControls.Movement.ReadValue<Vector2>();
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
        float rotY = transform.rotation.eulerAngles.y + horizontalLook;
        if (movePosition == Vector2.zero)
        {           
            camTransform.localEulerAngles = new Vector3(camControlX, rotY, 0f);
        }
        else
        {
            float rotX;
            if (camControlX < -87 || camControlX > 87)
            {
                rotX = camControlX;              
            }
            else
            {
                rotX = Mathf.LerpAngle(camTransform.rotation.eulerAngles.x, camControlX + (movePosition.y * SlantingPower), Time.fixedDeltaTime * SlantingSpeed);
            }          
            float rotZ = Mathf.LerpAngle(camTransform.rotation.eulerAngles.z, movePosition.x * SlantingPower, Time.fixedDeltaTime * SlantingSpeed);
            camTransform.localEulerAngles = new Vector3(rotX, rotY, rotZ);
        }   
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + horizontalLook, 0);
    }

    private void Jump()
    {
        if (inputs.BasicControls.Jump.ReadValue<float>() == 1)
        {
            if (GroundCheck())
            {
                if (!isJumping)
                {                  
                    if (RB.velocity.y > 1)
                    {
                        RB.AddForce(transform.up * (JumpForce / 2), ForceMode.Impulse);
                    }
                    else
                    {
                        RB.AddForce(transform.up * JumpForce, ForceMode.Impulse);
                    }
                    isJumping = true;
                }
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
        if (hit.collider)
        {
            isJumping = false;
            return true;
        }
        else return false;
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
        if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive)
        {
            var ray = new Ray(Helpers.MainCam.transform.position, Helpers.MainCam.transform.forward); //Camera.main.ScreenPointToRay(input.GetMousePosition());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 35, npcLayer)) //Interact with NPC
            {
                if (hit.transform.TryGetComponent<NPCBrain>(out selectedNPC))
                {
                    if (!gameController.IsDialogueActive)
                    {
                        dialogueManager.Highlight(selectedNPC.transform.position);
                        if (inputs.BasicControls.Interact.triggered)
                        {
                            selectedNPC.StartConversion();
                        }
                    }
                }
            }
            else
            {
                if (selectedNPC != null)
                {
                    dialogueManager.UnHighlight();
                }
            }
            if (gameController.IsDialogueActive || gameController.IsInventoryActive || gameController.IsMainMenuActive)
            {
                if (selectedNPC != null)
                {
                    dialogueManager.UnHighlight();
                }
            }
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 20, pickableLayer)) //Interact with Inventory items
            {
                if (hit.transform.TryGetComponent<PickedObject>(out selectedPickedObject))
                {
                    selectedPickedObject.HighlightObject();
                    if (inputs.BasicControls.Interact.triggered)//input.GetPickItems())
                    {
                        if (mainInventory.TryAddingItem(selectedPickedObject.GetItemSO()))
                        {
                            selectedPickedObject.DestroySelf();
                        }
                        else
                        {
                            selectedPickedObject.AddForceToItemSpawn(GetRandomDirWithoutY(2f, -2f));
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
        else if (gameController.IsDialogueActive || gameController.IsInventoryActive || gameController.IsMainMenuActive)
        {
            if (selectedNPC != null)
            {
                dialogueManager.UnHighlight();
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

    private void SceneLoader_OnNewGameStart()
    {
        transform.position = originalPlayerPos;
        gameObject.SetActive(true);
        audioListener.enabled = true;
        StartCoroutine(InputDone());
        AssignInv();
        AssignGameControl();
        AssignDialogueManager();
        GetComponent<Target>().SetupMaxHP(MaxHitPoints);
        GetComponent<Target>().SetupMaxMana(maxMana);
        CurrentHP = maxHitPoints;
        CurrentMana = maxMana;
        spellManager.SetupSpellCircle();
    }

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        gameObject.SetActive(false);
        weaponManager.DisableAllWeapons();
        audioListener.enabled = false;
    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        gameObject.SetActive(true);
    }

    private void Resource_OnManaLoss(object sender, ResourceManagement.ManaEventArgs e)
    {
        CurrentMana = e.CurrentMana;
    }

    private void Resource_OnManaGain(object sender, ResourceManagement.ManaEventArgs e)
    {
        CurrentMana = e.CurrentMana;
    }

    private void Resource_OnHealthLoss(object sender, ResourceManagement.DamagedEventArgs e)
    {
        CurrentHP = e.CurrentHP;
    }

    private void Resource_OnHealthGain(object sender, ResourceManagement.DamagedEventArgs e)
    {
        CurrentHP = e.CurrentHP;
    }
}