using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    #region Variables

    [SerializeField] private SpawnPortal portalPrefab;
    [SerializeField] private float spawnDistanceFromPortal;
    [SerializeField][Tooltip("Count of this should match with spawnItems")] private List<Vector3> portalPositions;
    [SerializeField][Tooltip("Count of this should match with portalPositions")] private List<SpawnItems> SpawnItems;
    private int amount = 0;

    #endregion

    #region General

    private void Awake()
    {
        foreach (var item in SpawnItems)
        {
            amount += item.Amount;
        }
    }

    private void Start()
    {
        if (portalPositions.Count != SpawnItems.Count)
        {
            Debug.LogError($"Number of Spawn items and portal positions doesn't match");
        }       
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(3);
            SpawnPortals();
        }
    }

    #endregion

    #region Mechanics

    public void SpawnPortals()
    {
        for (int i = 0; i < portalPositions.Count; i++)
        {
            var portal = Instantiate(portalPrefab, portalPositions[i], Quaternion.identity);
            portal.PortalIndex = i;
            portal.OnDestroyPortal += Portal_OnDestroyPortal;
            StartCoroutine(portal.PlaySpawnVFX(amount));
        }      
    }

    private void SpawnEnemies(int index)
    {
        var spawnItem = SpawnItems[index];
        var portalPos = portalPositions[index];
        var enemyPrefab = AssetLoader.GetEnemySOFromList(spawnItem.EnemyType).WorldPrefab;
        var enemyScale = AssetLoader.GetEnemySOFromList(spawnItem.EnemyType).GameWorldScale;
        for (int i = 1; i <= spawnItem.Amount; i++)
        {
            var enemy = Instantiate(enemyPrefab, new Vector3(
                portalPos.x + Random.Range(0f, 1f) * spawnDistanceFromPortal,
                spawnItem.NavmeshHeight + 10,
                portalPos.z + Random.Range(0f, 1f) * spawnDistanceFromPortal
                ), Quaternion.identity);
            enemy.gameObject.LeanScale(enemyScale, 0.2f);
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~

    #region Callback 

    private void Portal_OnDestroyPortal(SpawnPortal portal)
    {
        SpawnEnemies(portal.PortalIndex);
        portal.OnDestroyPortal -= Portal_OnDestroyPortal;
    }

    #endregion
}

[System.Serializable]
public class SpawnItems
{
    public int Amount;
    public EnemyTypes EnemyType;
    public float NavmeshHeight;
}