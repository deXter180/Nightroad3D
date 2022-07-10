using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LighteningBounce : MonoBehaviour
{
    [SerializeField] private float vfxduration = 0.5f;
    private float elapseTime;
    private bool isTagged;
    private Vector3 midPos1;
    private Vector3 midPos2;
    private string enemyTag = "Enemy";
    private string vfxAssetName = "ChainLightening2_vfx";
    private string startPosName = "StartPos";
    private string midPos1Name = "MidPos1";
    private string midPos2Name = "MidPos2";
    private string endPosName = "EndPos";
    private VisualEffect visualEffect;
    private SpellBaseSO spell;

    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    private void OnEnable()
    {
        isTagged = false;
        elapseTime = 0;
        spell = GameController.GetSpellSOFromList(SpellTypes.ChainLightening);
    }

    private void Update()
    {
        elapseTime += Time.deltaTime;
        if (elapseTime >= 2)
        {
            AssetLoader.ReleaseAssetInstance(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!isTagged)
        {
            if (col.CompareTag(enemyTag))
            {
                var eb = col.GetComponentInParent<EnemyBrain>();
                if (eb != null && !eb.IsHitByLightening)
                {
                    isTagged = true;
                    eb.SetLighteningHit();
                    BounceToTarget(eb.EnemyTransform.position);
                    spell.DoAttack(eb.GetComponent<Target>(), eb.ThisEnemySO.DodgeChance);                   
                    AssetLoader.CreateGOAsset(vfxAssetName, eb.EnemyTransform);
                    StartCoroutine(End());
                }
            }
        }
        IEnumerator End()
        {
            yield return Helpers.GetWait(vfxduration);
            AssetLoader.ReleaseAssetInstance(this.gameObject);
        }
    }

    public void BounceToTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(targetPos, transform.position);
        Ray ray = new Ray(transform.position, direction);
        midPos1 = ray.GetPoint(distance / 3);
        midPos2 = ray.GetPoint(distance * 2 / 3);
        visualEffect.SetVector3(startPosName, transform.position);
        visualEffect.SetVector3(midPos1Name, midPos1);
        visualEffect.SetVector3(midPos2Name, midPos2);
        visualEffect.SetVector3(endPosName, targetPos);
        visualEffect.Play();        
    }
}
