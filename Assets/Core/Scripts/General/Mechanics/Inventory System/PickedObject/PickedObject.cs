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
    private static float throwDistance = 5f;

    #endregion

    #region Properties

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

    public static PickedObject SpawnItemsWorld(ItemTypes itemType, InventoryItemSO SO, Vector3 position, WeaponTypes weaponType = WeaponTypes.None, ArmorTypes armorType = ArmorTypes.None, SpellTypes spellType = SpellTypes.None)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = null;
        if (weaponType != WeaponTypes.None && itemType == ItemTypes.Ammo)
        {
            spawnedTransform = Instantiate(GameController.GetInventoryItemSOFromList(itemType, weaponType).WorldPrefab, PlayerController.Instance.GetRandomDirWithoutY(throwDistance, -throwDistance) + groundPos, Quaternion.identity);
        }
        else
        {
            spawnedTransform = Instantiate(GameController.GetInventoryItemSOFromList(itemType).WorldPrefab, PlayerController.Instance.GetRandomDirWithoutY(throwDistance, -throwDistance) + groundPos, Quaternion.identity);
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

    public static PickedObject SpawnWeaponWorld(WeaponTypes weaponType, InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(GameController.GetWeaponInventorySO(weaponType).WorldPrefab, PlayerController.Instance.GetRandomDirWithoutY(throwDistance, -throwDistance) + groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        pickedObject.SetupInGameWorld(ItemTypes.Weapon, weaponType);
        return pickedObject;
    }

    public static PickedObject SpawnSpellWorld(SpellTypes spellType, InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(GameController.GetSpellInventorySO(spellType).WorldPrefab, PlayerController.Instance.GetRandomDirWithoutY(throwDistance, -throwDistance) + groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        pickedObject.SetupInGameWorld(ItemTypes.Spell, spellType);
        return pickedObject;
    }

    public static PickedObject SpawnArmorWorld(ArmorTypes armorType, InventoryItemSO SO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        Transform spawnedTransform = Instantiate(GameController.GetArmorInventorySO(armorType).WorldPrefab, PlayerController.Instance.GetRandomDirWithoutY(throwDistance, -throwDistance) + groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.itemSO = SO;
        pickedObject.SetupInGameWorld(ItemTypes.Armor, armorType);
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
            Transform spawnedTransform = Instantiate(GameController.GetInventoryItemSOFromList(itemType).WorldPrefab, new Vector3(posX, PlayerController.Instance.GroundHeight, posZ) + randomDir * 100f, Quaternion.identity);
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
            SR.material = GameController.GetMaterail(hightlightMaterial);
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
