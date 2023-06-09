using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedObject : MonoBehaviour
{
    #region Variables

    private string hightlightMaterial = "Outline_SpriteLit";
    private BoxCollider col;
    private Rigidbody RB;
    private SpriteRenderer SR;
    private Material defaultMaterial;
    private ItemTypes item;
    private WeaponTypes weapon;
    private SpellTypes spell;
    private ArmorTypes armor;
    private InventoryItemSO itemSO;

    #endregion

    #region Properties

    public Bounds RendererBound { get; private set; }
    public WeaponTypes WeaponTypes { get => weapon; }
    public SpellTypes SpellType { get => spell; }
    public ItemTypes ItemType { get => item; }

    #endregion

    #region General

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        RB = GetComponent<Rigidbody>();
        SR = GetComponentInChildren<SpriteRenderer>();        
    }

    private void Start()
    {
        if (SR != null)
        {
            RendererBound = SR.bounds;
            defaultMaterial = SR.material;
        }        
    }

    private void OnEnable()
    {
        col.isTrigger = true;
        RB.isKinematic = true;
    }

    #endregion

    #region Mechanics

    public InventoryItemSO GetItemSO()
    {
        return itemSO;
    }

    public static PickedObject SpawnItemsWorld(InventoryItemSO SO, Vector3 position, WeaponTypes weaponType = WeaponTypes.None, ArmorTypes armorType = ArmorTypes.None, SpellTypes spellType = SpellTypes.None)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = null;
        var itemType = SO.ItemType;
        if (weaponType != WeaponTypes.None && itemType == ItemTypes.Ammo)
        {
            spawnedTransform = Instantiate(SO.WorldPrefab, groundPos, Quaternion.identity);
        }
        else
        {
            spawnedTransform = Instantiate(SO.WorldPrefab, groundPos, Quaternion.identity);
        }
        if (spawnedTransform != null)
        {
            PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
            pickedObject.itemSO = SO;
            if (weaponType != WeaponTypes.None)
            {
                pickedObject.SetupInGameWorld(itemType, weaponType);
            }
            else
            {
                pickedObject.SetupInGameWorld(itemType);
            }            
            return pickedObject;
        }
        return null;
    }

    public static PickedObject SpawnWeaponWorld(InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(SO.WorldPrefab, groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        var type = SO.WeaponType;
        pickedObject.SetupInGameWorld(ItemTypes.Weapon, type);
        return pickedObject;
    }

    public static PickedObject SpawnSpellWorld(InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(SO.WorldPrefab, groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        var type = SO.SpellType;
        pickedObject.SetupInGameWorld(ItemTypes.Spell, type);
        return pickedObject;
    }

    public static PickedObject SpawnArmorWorld(InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(SO.WorldPrefab, groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        var type = SO.ArmorType;
        pickedObject.SetupInGameWorld(ItemTypes.Armor, type);
        return pickedObject;
    }

    public static PickedObject[] SpawnItemsWorldDrop(ItemTypes itemType, int Amount, Vector3 position)
    {
        PickedObject[] pickedObjects = new PickedObject[Amount];
        for (int i = 0; i < Amount; i++)
        {
            Vector3 randomDir = PlayerController.Instance.GetRandomDirWithoutY(1f, -1f);
            float posX = position.x;
            float posZ = position.z;
            Transform spawnedTransform = Instantiate(AssetLoader.GetInventoryItemSOFromList(itemType).WorldPrefab, new Vector3(posX, PlayerController.Instance.GroundHeight, posZ), Quaternion.identity);
            pickedObjects[i] = spawnedTransform.GetComponent<PickedObject>();
            pickedObjects[i].AddForceToItemSpawn(randomDir);
            pickedObjects[i].SetupInGameWorld(itemType, WeaponTypes.None);
        }
        return pickedObjects;
    }

    public void AddForceToItemSpawn(Vector3 randomDir)
    {
        GetThisRB().isKinematic = false;
        GetThisRB().AddForce(randomDir * 10f, ForceMode.Impulse);
        StartCoroutine(CheckForEndMovement());
    }

    private IEnumerator CheckForEndMovement()
    {
        yield return new WaitUntil(IsVelocityZero);
        GetThisRB().isKinematic = true;
    }

    private bool IsVelocityZero()
    {
        if (RB.velocity == Vector3.zero)
        {
            return true;
        }
        else return false;
    }

    private void SetupInGameWorld(ItemTypes item)
    {
        this.item = item;
    }

    private void SetupInGameWorld(ItemTypes item, WeaponTypes weaponType)
    {
        this.item = item;
        this.weapon = weaponType;
    }

    private void SetupInGameWorld(ItemTypes item, SpellTypes spellType)
    {
        this.item = item;
        this.spell = spellType;
    }

    private void SetupInGameWorld(ItemTypes item, ArmorTypes armorType)
    {
        this.item = item;
        this.armor = armorType;
    }

    public Rigidbody GetThisRB()
    {
        return RB;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void HighlightObject()
    {
        if (SR != null)
        {
            SR.material = AssetLoader.GetMaterail(hightlightMaterial);
        }
        
    }

    public void UnhighlightObject()
    {
        if (SR != null)
        {
            SR.material = defaultMaterial;
        }        
    }

    #endregion

}
