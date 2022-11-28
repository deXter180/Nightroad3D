using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpellManager : Singleton<SpellManager>
{
    #region Variables

    [SerializeField] private int SpellCount;
    [SerializeField] private float selectionRange;
    [SerializeField] private GameObject selectionCircle;
    public Transform FirePoint;
    [SerializeField] private List<STSpells> SpellVFXList;
    [SerializeField] private List<CastEffect> CastVFXList;   
    private VisualEffect CastVfx;   
    private Color castColor;
    private Transform spellCircle;
    private Vector3 AOEPosition;
    private PlayerController player;
    private WeaponManager weaponManager;
    private GameController gameController;
    private bool isInSpellCastMode;
    public bool IsInSpellCastMode { get => isInSpellCastMode; }
    private int SelectedSpellIndex = 0;
    private Spells spell = null;
    private bool inRange;
    private int bitmask = 1 << 8 | 1 << 7;
    public bool IsCastingSpell { get; set; }
    private PlayerInputAsset inputs;
    private bool IsInitialized => spellDict != null;
    public static Dictionary<SpellTypes, Spells> spellDict;
    private SpellCastState state;
    public static event Action<SpellTypes, int> OnAddingSpell;
    public static event Action<SpellTypes, int> OnRemovingSpell;

    #endregion

    #region General

    private enum SpellCastState
    {
        Default,
        SelectArea,
        Cast
    }

    protected override void Awake()
    {
        base.Awake();
        IsCastingSpell = false;
        isInSpellCastMode = false;
    }

    private void Start()
    {
        player = PlayerController.Instance;
        gameController = GameController.Instance;
        weaponManager = WeaponManager.Instance;
        StartCoroutine(InputDone());
        state = SpellCastState.Default;
        SetupSpellCircle();        
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        foreach (var tile in EquipMenuControl.SpellTileList)
        {
            tile.OnPlacedOnSpellMenu += Tile_OnPlacedOnSpellMenu;
            tile.OnRemovedFromSpellMenu += Tile_OnRemovedFromSpellMenu;
        }
        Initialize();
    }

    private void OnDestroy()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        foreach (var tile in EquipMenuControl.SpellTileList)
        {
            tile.OnPlacedOnSpellMenu -= Tile_OnPlacedOnSpellMenu;
            tile.OnRemovedFromSpellMenu -= Tile_OnRemovedFromSpellMenu;
        }
    }

    private void Update()
    {
        if (inputs != null && !player.IsPlayerDead)
        {
            if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive)
            {
                SpellSelect();
            }
        }
    }

    #endregion

    #region Mechanics

    private void Initialize()
    {
        if (!IsInitialized)
            spellDict = new Dictionary<SpellTypes, Spells>();
        else
            return;
    }

    private IEnumerator InputDone()
    {
        if (inputs == null)
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;            
        } 
    }

    private void SetupSpellCircle()
    {
        if (spellCircle == null && selectionCircle != null)
        {
            spellCircle = Instantiate(selectionCircle).transform;
            spellCircle.gameObject.SetActive(false);
        }        
    }

    private void AddSpell(SpellTypes type, SpellCategories category, int num)
    {
        Initialize();
        if (spellDict.Count < SpellCount)
        {
            Spells spell = null;
            if (category == SpellCategories.SingleTargetedProjectile)
            {
                spell = new SingleTargetedProjectile(type, category);
            }
            else if (category == SpellCategories.AOETargeted)
            {
                spell = new AOETargeted(type, category);
            }
            else if (category == SpellCategories.SelfTargeted)
            {
                spell = new SelfTargeted(type, category);
            }
            else if (category == SpellCategories.SingleTargetedRaycast)
            {
                spell = new SingleTargeted(type, category);
            }
            if (spell != null)
            {
                spellDict.Add(type, spell);
                OnAddingSpell?.Invoke(type, num);
            }
        }
    }

    private void RemoveSpell(SpellTypes type, int num)
    {
        if (spellDict.Count > 0 && spellDict.ContainsKey(type))
        {
            spellDict.Remove(type);
            OnRemovingSpell?.Invoke(type, num);
        }
    }

    private void SpellSelect()
    {
        Initialize();
        if (spellDict.Count >= 1 && !IsCastingSpell)
        {
            switch (state)
            {
                case SpellCastState.Default:
                    {
                        if (inputs.BasicControls.CastSpell1.triggered)
                        {
                            SelectedSpellIndex = 0;
                            spell = GetSpell();
                            if (spell != null && spell.ThisSpellSO.ManaCost <= player.CurrentMana)
                            {
                                if (spell.SpellCategory == SpellCategories.AOETargeted)
                                {
                                    state = SpellCastState.SelectArea;
                                }
                                else
                                {
                                    state = SpellCastState.Cast;
                                }
                            }
                        }
                        else if (inputs.BasicControls.CastSpell2.triggered && spellDict.Count > 1)
                        {
                            SelectedSpellIndex = 1;
                            spell = GetSpell();
                            if (spell != null && spell.ThisSpellSO.ManaCost <= player.CurrentMana)
                            {
                                if (spell.SpellCategory == SpellCategories.AOETargeted)
                                {
                                    state = SpellCastState.SelectArea;
                                }
                                else
                                {
                                    state = SpellCastState.Cast;
                                }
                            }
                        }
                    }
                    break;
                case SpellCastState.SelectArea:
                    {
                        if (spell != null)
                        {
                            DrawAreaSelection();
                        }
                    }
                    break;
                case SpellCastState.Cast:
                    {
                        if (spell != null)
                        {
                            IsCastingSpell = true;
                            if (spell.SpellCategory != SpellCategories.SelfTargeted)
                            {
                                weaponManager.DisableSelectedWeapon();
                                player.PlaySpellCastAnim(true);
                                GetCastColor(spell.SpellType);
                                GetCastVfx(spell.ThisSpellSO.CastVfxType);
                                CastVfx.SetVector4("Color", castColor);
                                CastVfx.Play();
                            }
                            if (spell.SpellCategory == SpellCategories.AOETargeted)
                            {
                                AOETargeted aoeSpell = (AOETargeted)spell;
                                aoeSpell.SetSpellPos(AOEPosition);
                            }
                            spell.CastSpell(() =>
                            {
                                player.PlayerTarget.UseMana(spell.ThisSpellSO.ManaCost);
                                state = SpellCastState.Default;
                                StartCoroutine(CastDelay());
                            });
                            
                        }
                    }
                    break;
            }
        }
        IEnumerator CastDelay()
        {
            yield return Helpers.GetWait(0.5f);
            player.PlaySpellCastAnim(false);      
            if (weaponManager.SelectedWeaponBrain != null)
            {
                StartCoroutine(weaponManager.EnableSelectedWeapon(() => {
                    IsCastingSpell = false;
                    isInSpellCastMode = false;
                }));
            }
            else
            {
                IsCastingSpell = false;
                isInSpellCastMode = false;
            }            
        }
    }   

    private Spells GetSpell()
    {
        int i = 0;
        foreach (var spell in spellDict.Values)
        {
            if (i == SelectedSpellIndex)
            {
                return spell;
            }
            i++;
        }
        return null;
    }

    public VisualEffect GetSTSpellVfx(SpellTypes spellType)
    {
        foreach (var vfx in SpellVFXList)
        {
            if (vfx.spellType == spellType)
            {
                return vfx.vfx;
            }
        }
        return null;
    }

    private void GetCastVfx(SpellCastTypes castType)
    {
        foreach (var vfx in CastVFXList)
        {
            if (vfx.castType == castType)
            {
                CastVfx = vfx.vfx;
            }
        }
    }

    private void DrawAreaSelection()
    {
        isInSpellCastMode = true;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 destination = new Vector3();
        Quaternion rotation = new Quaternion();
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, selectionRange, bitmask))
        {
            inRange = true;
            destination = hit.point;
            rotation = Quaternion.LookRotation(ray.direction);
        }
        else
        {
            inRange = false;
        }
        if (inRange)
        {
            spellCircle.position = destination + new Vector3(0, 1f, 0);
            rotation = Quaternion.Euler(spellCircle.transform.rotation.x, rotation.y, spellCircle.transform.rotation.z);
            spellCircle.rotation = rotation;
            spellCircle.gameObject.SetActive(true);
            if (inputs.BasicControls.Shoot.triggered)
            {
                AOEPosition = spellCircle.position;
                spellCircle.gameObject.SetActive(false);
                state = SpellCastState.Cast;
            }
            else if (inputs.BasicControls.CastSpell1.triggered || inputs.BasicControls.CastSpell2.triggered || inputs.UI.Menu.triggered)
            {
                spellCircle.gameObject.SetActive(false);
                state = SpellCastState.Default;
            }
        }
        else
        {
            spellCircle.gameObject.SetActive(false);
        }
        if (inputs.UI.Menu.triggered)
        {
            state = SpellCastState.Default;
        }
    }

    private void GetCastColor(SpellTypes spellType)
    {
        switch (spellType)
        {
            case SpellTypes.FreezeBlast:
                castColor = Color.cyan;
                break;
            case SpellTypes.FireBall:
                castColor = Color.yellow;
                break;
            case SpellTypes.ChainLightening:
                castColor = Color.HSVToRGB(201f, 0.731f, 1f, true);
                break;
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Tile_OnPlacedOnSpellMenu(PlacedObject e, int num)
    {
        if (e != null)
        {
            InventoryItemSO inventoryItem = e.GetInventoryItemSO();
            if (inventoryItem.ItemType == ItemTypes.Spell && inventoryItem.SpellType != SpellTypes.None && inventoryItem.SpellCategory != SpellCategories.None)
            {
                AddSpell(inventoryItem.SpellType, inventoryItem.SpellCategory, num);
            }
        }       
    }

    private void Tile_OnRemovedFromSpellMenu(PlacedObject e, int num)
    {
        if (e != null)
        {
            InventoryItemSO inventoryItem = e.GetInventoryItemSO();
            if (inventoryItem.ItemType == ItemTypes.Spell && inventoryItem.SpellType != SpellTypes.None && inventoryItem.SpellCategory != SpellCategories.None)
            {
                RemoveSpell(inventoryItem.SpellType, num);
            }
        }       
    }

    private void SceneLoader_OnNewGameStart()
    {
        spellDict.Clear();
        SetupSpellCircle();
    }

    #endregion
}


[Serializable]
public class STSpells
{
    public SpellTypes spellType;
    public VisualEffect vfx;
}

[Serializable]
public class CastEffect
{
    public SpellCastTypes castType;
    public VisualEffect vfx;
}
