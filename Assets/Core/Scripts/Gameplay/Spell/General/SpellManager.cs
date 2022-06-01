using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpellManager : Singleton<SpellManager>
{
    
    [SerializeField] private int SpellCount;
    [SerializeField] private float selectionRange;
    [SerializeField] private Transform SelectionCircle;
    public Transform FirePoint;
    [SerializeField] private List<STSpells> SelfSpellList;
    [SerializeField] private List<CastEffect> CastVFXList;
    private VisualEffect CastVfx;   
    private Color castColor;
    private Transform spellCircle;
    private Vector3 AOEPosition;
    private PlayerController player;
    private GameController gameController;
    private bool isRemoved;
    private bool isInSpellCastMode;
    public bool IsInSpellCastMode { get => isInSpellCastMode; }
    private int SelectedSpellIndex = 0;
    private Spells spell = null;
    private bool inRange;
    private int bitmask = 1 << 8 | 1 << 7;
    public bool IsCastingSpell { get; set; }
    private PlayerInputAsset inputs;
    private bool IsInitialized => spellDict != null;
    private Dictionary<SpellTypes, Spells> spellDict;
    private SpellCastState state;
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
        StartCoroutine(InputDone());
        state = SpellCastState.Default;
        SetupSpellCircle();    
        isRemoved = false;
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
            gameController = GameController.Instance;
        }
    }

    public void SetupSpellCircle()
    {
        if (spellCircle == null)
        {
            spellCircle = Instantiate(SpellManager.Instance.SelectionCircle);
            spellCircle.gameObject.SetActive(false);
        }        
    }

    private void AddSpell(SpellTypes type, SpellCategories category)
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
            if (spell != null)
            {
                spellDict.Add(type, spell);
            }
        }
    }

    private void RemoveSpell(SpellTypes type)
    {
        if (spellDict.Count > 0 && spellDict.ContainsKey(type))
        {
            spellDict.Remove(type);
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
            IsCastingSpell = false;
            isInSpellCastMode = false;
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
        foreach (var vfx in SelfSpellList)
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
        }
    }

    //~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    private void Tile_OnPlacedOnSpellMenu(object sender, PlacedObject e)
    {
        InventoryItemSO inventoryItem = e.GetInventoryItemSO();
        if (inventoryItem.ItemType == ItemTypes.Spell && inventoryItem.SpellType != SpellTypes.None && inventoryItem.SpellCategory != SpellCategories.None)
        {
            AddSpell(inventoryItem.SpellType, inventoryItem.SpellCategory);
        }
    }

    private void Tile_OnRemovedFromSpellMenu(object sender, PlacedObject e)
    {
        InventoryItemSO inventoryItem = e.GetInventoryItemSO();
        if (inventoryItem.ItemType == ItemTypes.Spell && inventoryItem.SpellType != SpellTypes.None && inventoryItem.SpellCategory != SpellCategories.None)
        {
            RemoveSpell(inventoryItem.SpellType);
            isRemoved = true;
        }
    }

    private void SceneLoader_OnNewGameStart()
    {
        spellDict = new Dictionary<SpellTypes, Spells>();
    }
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
