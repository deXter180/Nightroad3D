using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : PersistentSingleton<PlayerController>
{
    //~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~

    #region Properties

    public float GroundHeight;
    public Transform PlayerTransform { get; private set; }
    public Transform CameraTransform => camTransform;
    public Target PlayerTarget => target;
    public PlayerInputAsset Inputs => inputs;
    public int MaxHitPoints => modifiedMaxHp;
    public int BaseHitPoints => maxHitPoints;
    public int BaseMana => maxMana;
    public int MaxMana => modifiedMaxMp;
    public int CurrentHP { get; private set; }
    public int CurrentMana { get; private set; }
    public float DodgeChace => dodgeChance;
    public bool IsCrouching => isCrouching;
    public bool IsJumping => isJumping;
    public bool IsPlayerDead => isDead;
    public Vector3 DashPos => dashPos;
    public Rigidbody PlayerRB => RB;

    #endregion

    #region Variables

    [SerializeField] private int maxHitPoints;
    [SerializeField] private int maxMana;
    [SerializeField] private float statToHPModifier;
    [SerializeField] private float statToMPModifier;
    [SerializeField] private float MoveSpeed = 20;
    //[SerializeField] private float MoveAcceleration = 14f;
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
    [SerializeField] private Animator CamAnimator;
    [SerializeField] private CameraShake.ShakeProperty CamShakeOnDeath;
    [SerializeField] private CameraShake.ShakeProperty CamShakeOnDamage;
    private Transform camTransform;
    private InventorySystem mainInventory;
    private AttributeManager attributeManager;
    private Animator animator;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private WeaponManager weaponManager;
    private SpellManager spellManager;
    private RecipeManager recipeManager;
    private DialogueManager dialogueManager;
    private StashHolder selectedStashHolder = null;
    private CraftStation selectedCraftStation = null;
    private PickedObject selectedPickedObject = null;
    private PickedRecipe selectedPickedRecipe = null;
    private NPCBrain selectedNPC = null;
    private static float globalGravity = -9.81f;
    private Rigidbody RB;
    private CapsuleCollider primCol;
    private CapsuleCollider secndCol;
    private Target target;
    private AudioListener audioListener;
    private CameraShake cameraShake;
    private CharacterAttribute VitalityAttribute;
    private CharacterAttribute SpiritAttribute;
    private Vector2 movePosition = Vector2.zero;
    private Vector3 gravity;
    private Vector3 dashPos;
    private Vector3 originalPlayerPos;
    private Vector3 ConstantDistFromPlayer;
    //private Vector3 playerVelocity = Vector3.zero;
    private Quaternion oldRot;
    private int modifiedMaxHp = 0;
    private int modifiedMaxMp = 0;
    private int DOTAmount = 0;
    private int ground = 1 << 8;
    private int water = 1 << 4;
    private int pickableLayer = 1 << 6;
    private int npcLayer = 1 << 14;
    private int stashLayer = 1 << 15;
    private int craftLayer = 1 << 17;
    private int bitmask;
    private float camControlX = 0f;
    private float normalHeightPrim;
    private float normalHeightSec;
    private float crouchHeightPrim;
    private float crouchHeightSec;
    private float currentTime = 0;
    private float elapseTime = 0;
    private float DOTelapsedTime = 0;
    private float DOTDuration = 0;
    private float gravityControl;
    private float moveSpeedOnWater;
    private bool isInitiated;
    private bool isHitByDOT;
    private bool isDOTActive;
    private bool isOnWater;
    private bool isReset;
    private bool isMoving;
    private bool isJumping;
    private bool isCrouching;
    private bool isDead;
    private bool isDeathAnimFinish;
    private bool isToggledCrouching;
    private bool isCursorLocked;
    private string waterLayer = "Water";
    private string fricColName = "NoFriction";
    private int castHash = Animator.StringToHash("IsCasting");
    public static event Action OnPlayerDeath;
    public event Action onResettingHP;

    #endregion

    //~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~

    #region GeneralFunctions

    protected override void Awake()
    {
        base.Awake();
        if (GetComponentInChildren<AudioManager>())
        {
            Destroy(GetComponentInChildren<AudioManager>().gameObject);
        }
        RB = GetComponent<Rigidbody>();
        target = GetComponent<Target>();
        animator = GetComponent<Animator>();
        PlayerTransform = transform;
        animator.enabled = false;
        weaponManager = GetComponentInChildren<WeaponManager>();
        spellManager = GetComponentInChildren<SpellManager>();
        primCol = transform.Find("PrimaryCollider").GetComponent<CapsuleCollider>();
        secndCol = transform.Find("SecondCollider").GetComponent<CapsuleCollider>();
        bitmask = ground | water;
        moveSpeedOnWater = (float)Math.Round(MoveSpeed / 3, 2);
        isOnWater = false;
        isMoving = false;
        isDead = false;
        isDeathAnimFinish = false;
        isHitByDOT = false;
        isDOTActive = false;
        isInitiated = false;
    }

    private void Start()
    {
        modifiedMaxHp = maxHitPoints;
        modifiedMaxMp = maxMana;
        oldRot = Quaternion.identity;
        camTransform = FPSCamControl.Instance.transform;//Helpers.MainCam.transform;
        ConstantDistFromPlayer = camTransform.position - transform.position;
        attributeManager = AttributeManager.Instance;
        recipeManager = RecipeManager.Instance;
        StartCoroutine(InputDone());
        AssignInv();
        AssignGameControl();
        AssignDialogueManager();
        audioListener = Helpers.MainCam.GetComponent<AudioListener>();
        RB.useGravity = false;
        normalHeightPrim = primCol.height;
        normalHeightSec = secndCol.height;
        crouchHeightPrim = (normalHeightPrim * crouchHeightPrecentage) / 100;
        crouchHeightSec = (normalHeightSec * crouchHeightPrecentage) / 100;
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        target.Resource.OnHealthGain += Resource_OnHealthGain;
        target.Resource.OnHealthLoss += Resource_OnHealthLoss;
        target.Resource.OnManaGain += Resource_OnManaGain;
        target.Resource.OnManaLoss += Resource_OnManaLoss;
        GameController.OnStashClose += GameController_OnStashClose;
        isReset = false;
        isJumping = false;
        isCursorLocked = false;
        originalPlayerPos = transform.position;
        cameraShake = CameraShake.Instance;
    }

    private void FixedUpdate()
    {
        if (RB != null)
        {
            if (RB.velocity.y > 30)
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

    private void OnDestroy()
    {
        //AssetLoader.OnGOCreated += AssetLoader_OnGOCreated; //Delete this
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        target.Resource.OnHealthGain -= Resource_OnHealthGain;
        target.Resource.OnHealthLoss -= Resource_OnHealthLoss;
        target.Resource.OnManaGain -= Resource_OnManaGain;
        target.Resource.OnManaLoss -= Resource_OnManaLoss;
        GameController.OnStashClose -= GameController_OnStashClose;
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
                animator.enabled = true;
                animator.SetBool("IsDead", true);
                cameraShake.StartShake(CamShakeOnDeath);
            }
            if (!isDeathAnimFinish)
            {
                UpdateCameraPosition();
            }
        }
        if (isHitByDOT)
        {
            DOTelapsedTime += Time.deltaTime;
            if (DOTelapsedTime <= DOTDuration)
            {
                if (!isDOTActive)
                {
                    StartCoroutine(TakeDamegeOverTime());
                }
            }
            else
            {
                isHitByDOT = false;
                DOTelapsedTime = 0;
                DOTDuration = 0;
                DOTAmount = 0;
            }
        }
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
            if (!gameController.IsUIActive)
            {
                Move();
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

    public void PlaySpellCastAnim(bool isTrue)
    {
        CamAnimator.SetBool(castHash, isTrue);
    }

    #endregion

    //~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~~~

    #region MechanicsFunctions

    private void SetGravity()
    {
        gravity = globalGravity * gravityControl * Vector3.up;
        RB.AddForce(gravity, ForceMode.Impulse);
    }

    private void UpdateCameraPosition()
    {
        if (!isCrouching)
        {
            camTransform.position = transform.position + ConstantDistFromPlayer;
        }
        else
        {
            var pos = transform.position + ConstantDistFromPlayer;
            camTransform.position = pos + new Vector3 (0, -2f, 0);
        }
    }

    public void UpdateResouce(AttributeTypes type)
    {
        if (type == AttributeTypes.Vitality)
        {
            target.SetupMaxHP(GetModifiedHP());
        }
        else if (type == AttributeTypes.Spirit)
        {
            target.SetupMaxMana(GetModifiedMP());
        }
    }

    public int GetModifiedHP()
    {
        modifiedMaxHp += Mathf.RoundToInt(attributeManager.VitalityStat * statToHPModifier);
        return modifiedMaxHp;
    }

    public int GetModifiedMP()
    {
        modifiedMaxMp += Mathf.RoundToInt(attributeManager.SpiritStat * statToMPModifier);
        return modifiedMaxMp;
    }

    public void ResetHP(int value)
    {
        modifiedMaxHp = value;
        target.SetupMaxHP(modifiedMaxHp);
        onResettingHP?.Invoke();
    }

    public void ResetMP(int value)
    {
        modifiedMaxMp = value;
        target.SetupMaxHP(modifiedMaxMp);
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
            //Accelerate(wishDir, wishSpeed, MoveAcceleration);
            //playerVelocity.y = 0;          
            dashPos = transform.right * moveX + transform.forward * moveY;
            Vector3 movePos = Vector3.zero;
            if (isOnWater)
            {
                movePos = transform.position + dashPos * Time.fixedDeltaTime * moveSpeedOnWater;
            }
            else
            {
                //movePos = transform.position + (dashPos * Time.fixedDeltaTime) + playerVelocity;
                movePos = transform.position + dashPos * Time.fixedDeltaTime * MoveSpeed;
            }
            RB.MovePosition(movePos);
            isMoving = true;
        }
        //else
        //{
        //    playerVelocity = Vector3.zero;
        //}
    }

    //private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    //{
    //    float addspeed;
    //    float accelspeed;
    //    float currentspeed;

    //    playerVelocity.Normalize();
    //    currentspeed = Vector3.Dot(playerVelocity, wishdir);
    //    addspeed = wishspeed - currentspeed;
    //    if (addspeed <= 0)
    //        return;
    //    accelspeed = accel * Time.deltaTime * wishspeed;
    //    if (accelspeed > addspeed)
    //        accelspeed = addspeed;
    //    playerVelocity.x += accelspeed * wishdir.x;
    //    playerVelocity.z += accelspeed * wishdir.z;
    //}

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
                    primCol.height = crouchHeightPrim;
                    secndCol.height = crouchHeightSec;
                }
            }
            else
            {
                if (isCrouching)
                {
                    isCrouching = false;
                    primCol.height = normalHeightPrim;
                    secndCol.height = normalHeightSec;
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
                    primCol.height = crouchHeightPrim;
                    secndCol.height = crouchHeightSec;
                }
                else
                {
                    isCrouching = false;
                    isToggledCrouching = false;
                    primCol.height = normalHeightPrim;
                    secndCol.height = normalHeightSec;
                }
            }
        }
    }

    private bool GroundCheck()
    {
        if (primCol != null)
        {
            Physics.BoxCast(primCol.bounds.center, transform.localScale / 2, Vector3.down, out RaycastHit hit, Quaternion.identity, primCol.bounds.extents.y, bitmask);
            if (hit.collider)
            {
                if (hit.collider.CompareTag(waterLayer))
                {
                    isOnWater = true;
                }
                else
                {
                    isOnWater = false;
                }
                isJumping = false;
                return true;
            }
        }
        return false;
    }

    public void ApplyKnockback()
    {
        RB.AddForce((transform.up - transform.forward) * 20, ForceMode.Impulse);
    }

    public void PostDeathAnim()
    {
        HeadUpDisplayHandler.Instance.ExecuteOnDeath();
        isDeathAnimFinish = true;
    }

    public void TriggerDOT(int damageAmount, float duration)
    {
        isHitByDOT = true;
        DOTAmount = damageAmount;
        DOTDuration = duration;
    }

    private IEnumerator TakeDamegeOverTime()
    {
        isDOTActive = true;
        yield return Helpers.GetWait(1);
        target.DoDamage(DOTAmount, 0);
        isDOTActive = false;
    }

    private void InteractInWorld()
    {
        if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive && !gameController.IsDialogueActive)
        {
            var ray = new Ray(Helpers.MainCam.transform.position, Helpers.MainCam.transform.forward); //Camera.main.ScreenPointToRay(input.GetMousePosition());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 20, npcLayer)) //Interact with NPC
            {
                if (hit.transform.TryGetComponent<NPCBrain>(out selectedNPC))
                {
                    gameController.HighlightInteract(selectedNPC.transform.position);
                    if (inputs.BasicControls.Interact.triggered)
                    {
                        selectedNPC.StartConversion();
                    }
                }
            }
            else
            {
                if (selectedNPC != null)
                {
                    gameController.UnHighlightInteract();
                    selectedNPC = null;
                }
            }
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 15, pickableLayer)) //Interact with Inventory items
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
                else if (hit.transform.TryGetComponent<PickedRecipe>(out selectedPickedRecipe))
                {
                    selectedPickedRecipe.HighlightObject();
                    if (inputs.BasicControls.Interact.triggered)
                    {
                        recipeManager.AddRecipe(selectedPickedRecipe.Recipe);
                        selectedPickedRecipe.DestroySelf();
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
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 20, stashLayer)) //Interact with Stash
            {
                if (hit.transform.TryGetComponent<StashHolder>(out selectedStashHolder))
                {
                    selectedStashHolder.Highlight(selectedStashHolder.transform.position);
                    if (inputs.BasicControls.Interact.triggered)
                    {
                        selectedStashHolder.LoadItemToStash();
                        gameController.OpenStash();
                    }
                }
            }
            else
            {
                if (selectedStashHolder != null)
                {
                    selectedStashHolder.Unhighlight();
                }
            }
            if (Physics.Raycast(ray, out hit, Helpers.MainCam.farClipPlane / 20, craftLayer))
            {
                if (hit.transform.TryGetComponent<CraftStation>(out selectedCraftStation))
                {
                    gameController.HighlightInteract(selectedCraftStation.transform.position);
                    if (inputs.BasicControls.Interact.triggered)
                    {
                        selectedCraftStation.EnterCrafting();
                    }
                }
            }
            else
            {
                if (selectedCraftStation != null)
                {
                    gameController.UnHighlightInteract();
                    selectedCraftStation = null;
                }
            }
        }
        if (gameController.IsDialogueActive || gameController.IsInventoryActive || gameController.IsMainMenuActive || gameController.IsStashActive || gameController.IsCraftingActive)
        {
            if (selectedNPC != null || selectedCraftStation != null)
            {
                gameController.UnHighlightInteract();
            }
            if (selectedStashHolder != null)
            {
                selectedStashHolder.Unhighlight();
            }
        }
    }

    #endregion

    #region UtilityFunctions

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

    private void ControlCursor()
    {
        if (!isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
    }

    public void ParentPlayerCam()
    {
        camTransform.parent = transform;
    }

    public void UnParentPlayerCam()
    {
        camTransform.parent = null;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void SceneLoader_OnNewGameStart()
    {
        if (isInitiated)
        {
            modifiedMaxHp = maxHitPoints;
            modifiedMaxMp = maxMana;
        }
        Target target = GetComponent<Target>();
        transform.position = originalPlayerPos;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        if (audioListener == null)
        {
            audioListener = Helpers.MainCam.GetComponent<AudioListener>();
        }
        audioListener.enabled = true;
        StartCoroutine(InputDone());
        AssignInv();
        AssignGameControl();
        AssignDialogueManager();
        attributeManager = AttributeManager.Instance;
        recipeManager = RecipeManager.Instance;
        recipeManager.Initialize();
        target.SetupMaxHP(modifiedMaxHp);       
        target.SetupMaxMana(modifiedMaxMp);
        CurrentHP = modifiedMaxHp;
        CurrentMana = modifiedMaxMp;
        isDead = target.IsDead = false;
        animator.SetBool("IsDead", false);
        animator.enabled = false;
        isInitiated = true;
    }

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        gameObject.SetActive(false);
        weaponManager.DisableAllWeapons();
        audioListener.enabled = false;
    }

    private void GameController_OnStashClose()
    {
        if (selectedStashHolder != null)
        {
            selectedStashHolder.UnloadItemFromStash();
            selectedStashHolder = null;
        }
    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        gameObject.SetActive(true);
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return Helpers.GetWait(0.15f);
            var portalMarker = FindObjectOfType<TestToMainLevel>();
            if (portalMarker != null)
            {
                transform.position = portalMarker.transform.position + new Vector3(0, 5, 0);
                UnParentPlayerCam();
            }
        }
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
        cameraShake.StartShake(CamShakeOnDamage);
    }

    private void Resource_OnHealthGain(object sender, ResourceManagement.DamagedEventArgs e)
    {
        CurrentHP = e.CurrentHP;
    }
    #endregion
}