using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedObject : MonoBehaviour
{
    private BoxCollider col;
    private Rigidbody RB;         
    private ItemTypes item;
    public ItemTypes itemType { get => item; }

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        RB = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        col.isTrigger = true;
        RB.isKinematic = true;
    }

    public static PickedObject SpawnItemWorld(ItemTypes itemType, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);        
        Transform spawnedTransform = Instantiate(AssetCollections.GetInventoryItemSOFromList(itemType).WorldPrefab, PlayerController.Instance.GetRandomDirection() + groundPos, Quaternion.identity);
        PickedObject pickedObject = spawnedTransform.GetComponent<PickedObject>();
        pickedObject.SetupInGameWorld(itemType);
        return pickedObject;
    }

    public static PickedObject[] SpawnItemsWorldDrop(ItemTypes itemType, int Amount, Vector3 position)
    {
        PickedObject[] pickedObjects = new PickedObject[Amount];       
        for (int i = 0; i < Amount; i++)
        {
            Vector3 randomDir = PlayerController.Instance.GetRandomDirection();
            float posX = position.x;
            float posZ = position.z;
            Transform spawnedTransform = Instantiate(AssetCollections.GetInventoryItemSOFromList(itemType).WorldPrefab, new Vector3(posX, PlayerController.Instance.GroundHeight, posZ) + randomDir * 100f, Quaternion.identity);           
            pickedObjects[i] = spawnedTransform.GetComponent<PickedObject>();
            pickedObjects[i].AddForceToItemSpawn(randomDir);
            pickedObjects[i].SetupInGameWorld(itemType);
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

    public InventoryItemSO GetItemSO()
    {
        return AssetCollections.GetInventoryItemSOFromList(item);
    }

    private void SetupInGameWorld(ItemTypes item)
    {
        this.item = item;
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

    }

    public void UnhighlightObject()
    {

    }

    
}
