using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public abstract class Spells
{
    protected string spellSizeText = "Size";
    protected SpellManager spellManager;
    protected AttributeManager attributeManager;
    protected SpellTypes spellType;
    protected SpellCategories spellCategory;
    protected SpellBaseSO spellSO;
    protected float modifiedValue = 0;
    public SpellBaseSO ThisSpellSO => spellSO;
    public SpellTypes SpellType { get => spellType; }
    public SpellCategories SpellCategory { get => spellCategory; }

    public Spells(SpellTypes spellType, SpellCategories category)
    {
        this.spellType = spellType;
        spellCategory = category;
        spellSO = GameController.GetSpellSOFromList(spellType);
        spellManager = SpellManager.Instance;
        attributeManager = AttributeManager.Instance;
    }

    public abstract void CastSpell(Action action);
    public abstract float GetModifiedStats();
}

public class SingleTargetedProjectile : Spells
{
    private ObjectPooler objectPooler;
    
    public static event EventHandler<OnSTSpellCastEventArg> OnProjectileSpellCast;

    public SingleTargetedProjectile(SpellTypes spellType, SpellCategories category) : base(spellType, category)
    {
        objectPooler = ObjectPooler.Instance;        
    }

    public override void CastSpell(Action action)
    {
        var shot = objectPooler.GetPooledObject(GetProjectile());
        if (shot != null)
        {
            var rotX = spellManager.FirePoint.rotation.eulerAngles.x + shot.transform.eulerAngles.x - 10f;
            var rotY = spellManager.FirePoint.rotation.eulerAngles.y;
            var rotZ = spellManager.FirePoint.rotation.eulerAngles.z;
            shot.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
            shot.transform.position = spellManager.FirePoint.position;
            shot.gameObject.SetActive(true);
            OnProjectileSpellCast?.Invoke(this, new OnSTSpellCastEventArg(this, spellType, spellCategory));
        }
        action.Invoke();
    }

    private ProjectileTypes GetProjectile()
    {
        switch (spellType)
        {
            case SpellTypes.FireBall:
                return ProjectileTypes.FireBallSpell;
            default:
                return ProjectileTypes.None;
        }
    }

    public void SpawnHitVfx(Vector3 HitPointPos)
    {
        AssetLoader.CreateAndReleaseAsset("Explosion_vfx", HitPointPos, 0.3f);
    }

    public override float GetModifiedStats()
    {
        modifiedValue = Mathf.RoundToInt(attributeManager.IntelligenceStat * spellSO.StatMultiplier);
        return spellSO.EffectAmount + modifiedValue;
    }
}

public class SingleTargeted : Spells
{
    protected float radius = 4f;
    protected int enemyLayer = 1 << 12;
    protected string vfxAssetName = "ChainLightening2_vfx";
    private PlayerController player;
    public SingleTargeted(SpellTypes spellType, SpellCategories category) : base(spellType, category)
    {
        player = PlayerController.Instance;
    }


    public override void CastSpell(Action action)
    {    
        if (spellManager.GetSTSpellVfx(spellType) != null)
        {
            VisualEffect vfx = spellManager.GetSTSpellVfx(spellType);
            vfx.Play();
        }
        if (Physics.SphereCast(player.PlayerTransform.position, radius, player.CameraTransform.forward, out RaycastHit hit, spellSO.Range, enemyLayer))
        {
            var eb = hit.collider.GetComponentInParent<EnemyBrain>();
            if (eb != null && !eb.IsHitByLightening)
            {
                eb.SetLighteningHit();
                spellSO.UpdateModifiedValue(GetModifiedStats());
                spellSO.DoAttack(eb.GetComponent<Target>(), eb.ThisEnemySO.DodgeChance);
                AssetLoader.CreateGOAsset(vfxAssetName, eb.EnemyTransform);
            }
        }
        action.Invoke();
    }

    public override float GetModifiedStats()
    {
        modifiedValue = (float)Math.Round(attributeManager.IntelligenceStat * spellSO.StatMultiplier, 2);
        return spellSO.EffectAmount + Mathf.RoundToInt(modifiedValue);
    }
}

public class AOETargeted : Spells
{
    private Vector3 spellPosition;
    public static event EventHandler<OnAOESpellCastEventArg> OnAOESpellCast;

    public AOETargeted(SpellTypes spellType, SpellCategories category) : base(spellType, category)
    {
        
    }

    public void SetSpellPos(Vector3 position)
    {
        spellPosition = position;
    }

    public override void CastSpell(Action action)
    {
        if (GetAssetName() != null)
        {        
            AssetLoader.CreateAndReleaseAsset(GetAssetName(), spellPosition, GetModifiedStats());
            OnAOESpellCast?.Invoke(this, new OnAOESpellCastEventArg(this, spellType, spellCategory));
            action.Invoke();
        }       
    }

    public IEnumerator AddSpellEffect(EnemyBrain enemyBrain, Action action)
    {
        switch (spellType)
        {
            case SpellTypes.FreezeBlast:
                {
                    yield return Helpers.GetWait(GetModifiedStats());
                }
                break;
        }
        action.Invoke();
    }

    private string GetAssetName()
    {
        string assetName = null;
        switch (spellType)
        {
            case SpellTypes.FreezeBlast:
                assetName = "AOEFrostEffect_vfx";
                break;
        }
        return assetName;
    }

    public override float GetModifiedStats()
    {
        modifiedValue = (float)Math.Round(attributeManager.IntelligenceStat * spellSO.StatMultiplier, 2);
        return spellSO.Duration + modifiedValue;
    }
}

public class SelfTargeted : Spells
{
    private PlayerController player;
    public SelfTargeted(SpellTypes spellType, SpellCategories category) : base(spellType, category)
    {
        player = PlayerController.Instance;
    }

    public override void CastSpell(Action action)
    {
        AddSpellEffect();
        action.Invoke();
    }

    public override float GetModifiedStats()
    {
        modifiedValue = (float)Math.Round(attributeManager.IntelligenceStat * spellSO.StatMultiplier, 2);
        return spellSO.Range + modifiedValue;
    }

    private void AddSpellEffect()
    {
        switch (spellType)
        {
            case SpellTypes.Dash:
                {                    
                    Vector3 direction;
                    if (player.DashPos.Equals(Vector3.zero))
                    {
                        direction = player.transform.forward;
                    }else
                    {
                        direction = player.DashPos;
                    }
                    Vector3 dir = player.transform.position + direction * GetModifiedStats();
                    player.PlayerRB.MovePosition(dir);
                    PlayDashVFX(spellType, direction);
                }
                break;
        }
    }

    private void PlayDashVFX(SpellTypes spellType, Vector3 direction)
    {
        if (spellManager.GetSTSpellVfx(spellType) != null)
        {
            VisualEffect vfx = spellManager.GetSTSpellVfx(spellType);
            vfx.transform.localRotation = Quaternion.Euler(0, 0, 0);
            vfx.SetFloat(spellSizeText, 0.04f);
            vfx.transform.localPosition = new Vector3(0, vfx.transform.localPosition.y, vfx.transform.localPosition.z);
            float temp1 = Vector3.Dot(direction, player.transform.forward);
            if (temp1 <= -0.5f || temp1 >= 0.5f)
            {
                vfx.Play();
            }
            else
            {
                float temp2 = Vector3.Dot(direction, player.transform.right);
                vfx.transform.localRotation = Quaternion.Euler(0, 90, 0);
                vfx.SetFloat(spellSizeText, 0.1f);
                if (temp2 >= 0.5f)
                {
                    vfx.transform.localPosition = new Vector3(0.4f, vfx.transform.localPosition.y, vfx.transform.localPosition.z);
                }
                else if (temp2 <= -0.5f)
                {
                    vfx.transform.localPosition = new Vector3(-0.4f, vfx.transform.localPosition.y, vfx.transform.localPosition.z);
                }
                vfx.Play();               
            }
        }
    }

   

}

public class OnSTSpellCastEventArg : EventArgs
{
    public SingleTargetedProjectile spell;
    public SpellTypes spellType;
    public SpellCategories spellCategory;

    public OnSTSpellCastEventArg(SingleTargetedProjectile SP, SpellTypes ST, SpellCategories SC)
    {
        spell = SP;
        spellType = ST;
        spellCategory = SC;
    }
}

public class OnAOESpellCastEventArg : EventArgs
{
    public AOETargeted spell;
    public SpellTypes spellType;
    public SpellCategories spellCategory;

    public OnAOESpellCastEventArg(AOETargeted SP, SpellTypes ST, SpellCategories SC)
    {
        spell = SP;
        spellType = ST;
        spellCategory = SC;
    }
}